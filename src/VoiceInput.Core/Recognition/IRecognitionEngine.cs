namespace VoiceInput.Core.Recognition;

public interface IRecognitionEngine
{
    string DisplayName { get; }

    Task<RecognitionResult> TranscribeAsync(
        string audioPath,
        RecognitionOptions options,
        CancellationToken cancellationToken);
}
