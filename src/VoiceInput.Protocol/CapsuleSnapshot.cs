namespace VoiceInput.Protocol;

public sealed record CapsuleSnapshot(
    CapsuleState State,
    string Message,
    string? PreviewText = null,
    TimeSpan? RecordingDuration = null);
