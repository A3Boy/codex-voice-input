namespace VoiceInput.Core.Recognition;

public sealed record RecognitionOptions(
    string? Language = null,
    bool PreferStreaming = false);
