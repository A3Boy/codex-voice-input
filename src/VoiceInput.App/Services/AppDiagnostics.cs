namespace VoiceInput.App.Services;

public static class AppDiagnostics
{
    private const long MaxLogBytes = 5 * 1024 * 1024;
    private static readonly object Lock = new();
    private static readonly string LogDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CodexVoiceInput");
    private static readonly string LogPath = Path.Combine(LogDirectory, "codex-voice-input.log");

    public static string FilePath => LogPath;

    public static void Info(string message) => Write("INFO", message);

    public static void Error(string message, Exception error) => Write("ERROR", $"{message}: {error}");

    private static void Write(string level, string message)
    {
        try
        {
            Directory.CreateDirectory(LogDirectory);
            lock (Lock)
            {
                var line = $"{DateTimeOffset.Now:O} [{level}] {message}{Environment.NewLine}";
                if (File.Exists(LogPath) && new FileInfo(LogPath).Length >= MaxLogBytes)
                {
                    File.WriteAllText(LogPath, line);
                }
                else
                {
                    File.AppendAllText(LogPath, line);
                }
            }
        }
        catch
        {
            // Diagnostics must never break voice input.
        }
    }
}
