using System.Text.Json;

namespace VoiceInput.App.Services;

public sealed class AppConfig
{
    public string Hotkey { get; set; } = "Ctrl+Alt+Space";
    public string? AuthFile { get; set; }
    public int AudioDeviceNumber { get; set; } = 0;
    public bool ShowAfterInput { get; set; } = true;
    public bool DarkMode { get; set; }

    public static string DirectoryPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CodexVoiceInput");

    public static string FilePath => Path.Combine(DirectoryPath, "config.json");

    public static AppConfig Load()
    {
        Directory.CreateDirectory(DirectoryPath);
        if (!File.Exists(FilePath))
        {
            var created = new AppConfig();
            created.Save();
            return created;
        }

        try
        {
            var config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(FilePath), JsonOptions());
            config ??= new AppConfig();
            config.Save();
            return config;
        }
        catch (Exception error)
        {
            AppDiagnostics.Error("Failed to load config; using defaults", error);
            return new AppConfig();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(DirectoryPath);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this, JsonOptions()));
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}
