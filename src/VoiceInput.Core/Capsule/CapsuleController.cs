using VoiceInput.Core.Recognition;
using VoiceInput.Core.Text;
using VoiceInput.Protocol;

namespace VoiceInput.Core.Capsule;

public sealed class CapsuleController(IRecognitionEngine recognitionEngine)
{
    private readonly CancellationTokenSource lifetime = new();
    private readonly object cancellationGate = new();
    private CancellationTokenSource? activeTranscription;
    private CapsuleSnapshot snapshot = new(CapsuleState.Idle, "点击开始语音输入");

    public event EventHandler<CapsuleSnapshot>? SnapshotChanged;

    public CapsuleSnapshot Snapshot => snapshot;

    public void StartRecording()
    {
        Publish(new CapsuleSnapshot(CapsuleState.Recording, "正在录音"));
    }

    public async Task StopAndTranscribeAsync(string audioPath)
    {
        Publish(new CapsuleSnapshot(CapsuleState.Transcribing, "正在识别语音..."));
        using var requestCancellation = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);
        lock (cancellationGate)
        {
            activeTranscription = requestCancellation;
        }

        try
        {
            var result = await recognitionEngine.TranscribeAsync(
                audioPath,
                new RecognitionOptions(),
                requestCancellation.Token);

            var processed = TextPostProcessor.Process(result.Text);
            Publish(new CapsuleSnapshot(CapsuleState.Ready, "识别完成，点击输入", processed));
        }
        catch (OperationCanceledException) when (requestCancellation.IsCancellationRequested)
        {
            if (snapshot.State == CapsuleState.Transcribing)
            {
                Reset();
            }
        }
        catch (Exception error)
        {
            Publish(new CapsuleSnapshot(CapsuleState.Error, ToDisplayError(error)));
        }
        finally
        {
            lock (cancellationGate)
            {
                if (ReferenceEquals(activeTranscription, requestCancellation))
                {
                    activeTranscription = null;
                }
            }
        }
    }

    public bool CancelTranscription()
    {
        CancellationTokenSource? cancellation;
        lock (cancellationGate)
        {
            cancellation = activeTranscription;
        }

        if (cancellation is null)
        {
            return false;
        }

        cancellation.Cancel();
        return true;
    }

    public void Reset() => Publish(new CapsuleSnapshot(CapsuleState.Idle, "点击开始语音输入"));

    private void Publish(CapsuleSnapshot next)
    {
        snapshot = next;
        SnapshotChanged?.Invoke(this, next);
    }

    private static string ToDisplayError(Exception error)
    {
        var message = error.Message
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Trim();
        if (message.Contains("transcribe request failed", StringComparison.OrdinalIgnoreCase)
            || message.Contains("backend-api/transcribe", StringComparison.OrdinalIgnoreCase))
        {
            return "识别失败：ChatGPT 登录已失效";
        }

        return string.IsNullOrWhiteSpace(message)
            ? "识别失败"
            : message.Length > 42
                ? message[..42] + "..."
                : message;
    }

}
