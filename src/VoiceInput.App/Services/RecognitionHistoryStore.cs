using System.Text.Json;

namespace VoiceInput.App.Services;

public sealed record RecognitionHistoryEntry(
    Guid Id,
    DateTimeOffset CreatedAt,
    string Text);

public sealed class RecognitionHistoryStore
{
    private const int MaximumEntries = 200;
    private readonly object gate = new();
    private List<RecognitionHistoryEntry> entries;

    public RecognitionHistoryStore()
    {
        entries = Load();
    }

    public event EventHandler? Changed;

    public static string FilePath => Path.Combine(AppConfig.DirectoryPath, "history.json");

    public IReadOnlyList<RecognitionHistoryEntry> Snapshot()
    {
        lock (gate)
        {
            return entries.ToArray();
        }
    }

    public void Add(string text)
    {
        text = text.Trim();
        if (text.Length == 0)
        {
            return;
        }

        lock (gate)
        {
            entries.Insert(0, new RecognitionHistoryEntry(Guid.NewGuid(), DateTimeOffset.Now, text));
            if (entries.Count > MaximumEntries)
            {
                entries.RemoveRange(MaximumEntries, entries.Count - MaximumEntries);
            }
            SaveLocked();
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        lock (gate)
        {
            entries.Clear();
            SaveLocked();
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private static List<RecognitionHistoryEntry> Load()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<RecognitionHistoryEntry>>(
                File.ReadAllText(FilePath),
                JsonOptions()) ?? [];
        }
        catch (Exception error)
        {
            AppDiagnostics.Error("Failed to load recognition history", error);
            return [];
        }
    }

    private void SaveLocked()
    {
        Directory.CreateDirectory(AppConfig.DirectoryPath);
        var temporaryPath = FilePath + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(entries, JsonOptions()));
        File.Move(temporaryPath, FilePath, overwrite: true);
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
