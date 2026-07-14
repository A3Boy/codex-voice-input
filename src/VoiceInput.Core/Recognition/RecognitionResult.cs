namespace VoiceInput.Core.Recognition;

public sealed record RecognitionResult(
    string Text,
    bool IsFinal = true,
    double? Confidence = null);
