using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace VoiceInput.Core.Recognition;

public sealed class CodexAsrRecognitionEngine : IRecognitionEngine
{
    private const string DefaultEndpoint = "https://chatgpt.com/backend-api/transcribe";
    private const string DefaultDesktopVersion = "26.707.8479.0";

    private readonly string authFile;
    private readonly Uri endpoint;

    public CodexAsrRecognitionEngine(string? authFile = null, string endpoint = DefaultEndpoint)
    {
        this.authFile = string.IsNullOrWhiteSpace(authFile)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".codex", "auth.json")
            : authFile;
        this.endpoint = new Uri(endpoint, UriKind.Absolute);
    }

    public string DisplayName => "Codex Desktop ASR";

    public async Task<RecognitionResult> TranscribeAsync(
        string audioPath,
        RecognitionOptions options,
        CancellationToken cancellationToken)
    {
        ValidateAudio(audioPath);
        var auth = LoadAuth(authFile);

        using var client = CreateHttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        request.Headers.TryAddWithoutValidation("originator", "Codex Desktop");
        request.Headers.UserAgent.ParseAdd(
            $"Codex Desktop/{DetectCodexDesktopVersion() ?? DefaultDesktopVersion} (Windows; x64)");
        if (!string.IsNullOrWhiteSpace(auth.AccountId))
        {
            request.Headers.TryAddWithoutValidation("ChatGPT-Account-Id", auth.AccountId);
        }

        await using var audio = File.OpenRead(audioPath);
        using var multipart = new MultipartFormDataContent();
        using var audioContent = new StreamContent(audio);
        audioContent.Headers.ContentType = new MediaTypeHeaderValue(InferContentType(audioPath));
        multipart.Add(audioContent, "file", SanitizeFilename(Path.GetFileName(audioPath)));
        if (HasExplicitLanguage(options.Language))
        {
            multipart.Add(new StringContent(options.Language!, Encoding.UTF8), "language");
        }
        request.Content = multipart;

        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(CreateHttpErrorMessage(response.StatusCode));
        }

        CodexAsrJson? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<CodexAsrJson>(body);
        }
        catch (JsonException error)
        {
            throw new InvalidOperationException($"Codex 转写返回了无法解析的 JSON：{Clip(body)}", error);
        }

        var text = parsed?.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("没有识别到文本");
        }

        return new RecognitionResult(text);
    }

    private static string CreateHttpErrorMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => "Codex 登录已失效，请重新登录",
            HttpStatusCode.TooManyRequests => "请求过于频繁，请稍后重试",
            _ => $"转写失败：HTTP {(int)statusCode}",
        };
    }

    private static HttpClient CreateHttpClient()
    {
        var configuredProxy = FirstNonEmpty(
            Environment.GetEnvironmentVariable("CODEX_ASR_PROXY"),
            Environment.GetEnvironmentVariable("HTTPS_PROXY"),
            Environment.GetEnvironmentVariable("https_proxy"));
        IWebProxy proxy = configuredProxy is null
            ? WebRequest.GetSystemWebProxy()
            : new WebProxy(configuredProxy);
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
            Proxy = proxy,
            UseProxy = true,
        };
        return new HttpClient(handler, disposeHandler: true)
        {
            Timeout = TimeSpan.FromMinutes(5),
        };
    }

    private static CodexAuth LoadAuth(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("找不到 Codex 登录文件", path);
        }

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        if (!document.RootElement.TryGetProperty("tokens", out var tokens)
            || !tokens.TryGetProperty("access_token", out var accessTokenValue)
            || string.IsNullOrWhiteSpace(accessTokenValue.GetString()))
        {
            throw new InvalidOperationException("Codex 登录文件中没有 ChatGPT access token");
        }

        var accessToken = accessTokenValue.GetString()!;
        var accountId = tokens.TryGetProperty("account_id", out var accountIdValue)
            ? accountIdValue.GetString()
            : null;
        accountId ??= AccountIdFromJwt(accessToken);
        return new CodexAuth(accessToken, accountId);
    }

    private static string? AccountIdFromJwt(string accessToken)
    {
        try
        {
            var parts = accessToken.Split('.');
            if (parts.Length < 2) return null;
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            using var document = JsonDocument.Parse(Convert.FromBase64String(payload));
            return document.RootElement
                .GetProperty("https://api.openai.com/auth")
                .GetProperty("chatgpt_account_id")
                .GetString();
        }
        catch
        {
            return null;
        }
    }

    private static string? DetectCodexDesktopVersion()
    {
        foreach (var process in Process.GetProcessesByName("ChatGPT"))
        {
            using (process)
            {
                try
                {
                    var executable = process.MainModule?.FileName;
                    var directory = executable is null ? null : Path.GetDirectoryName(executable);
                    var manifestPath = directory is null ? null : Path.Combine(directory, "AppxManifest.xml");
                    if (manifestPath is null || !File.Exists(manifestPath)) continue;
                    var identity = XDocument.Load(manifestPath)
                        .Descendants()
                        .FirstOrDefault(element => element.Name.LocalName == "Identity");
                    var version = identity?.Attribute("Version")?.Value;
                    if (!string.IsNullOrWhiteSpace(version)) return version;
                }
                catch
                {
                    // Some Chromium child processes do not expose their main module.
                }
            }
        }
        return null;
    }

    private static void ValidateAudio(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("录音文件不存在", path);
        if (new FileInfo(path).Length <= 44) throw new InvalidOperationException("录音内容为空");
    }

    private static string InferContentType(string path) =>
        Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".wav" or ".wave" => "audio/wav",
            ".webm" => "audio/webm",
            ".mp3" => "audio/mpeg",
            ".m4a" or ".mp4" => "audio/mp4",
            ".ogg" or ".oga" => "audio/ogg",
            ".flac" => "audio/flac",
            _ => "application/octet-stream",
        };

    private static string SanitizeFilename(string filename) => filename.Replace("\"", string.Empty);

    private static string? FirstNonEmpty(params string?[] values) =>
        values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim();

    private static bool HasExplicitLanguage(string? language) =>
        !string.IsNullOrWhiteSpace(language)
        && !language.Equals("auto", StringComparison.OrdinalIgnoreCase);

    private static string Clip(string value, int maxLength = 500)
    {
        value = value.Replace("\r", " ").Replace("\n", " ").Trim();
        return value.Length > maxLength ? value[..maxLength] + "..." : value;
    }

    private sealed record CodexAuth(string AccessToken, string? AccountId);

    private sealed class CodexAsrJson
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
