using System.Buffers.Binary;
using NAudio.Wave;

namespace VoiceInput.App.Services;

public sealed class WavAudioRecorder : IDisposable
{
    private readonly string tempDirectory;
    private readonly int deviceNumber;
    private WaveInEvent? waveIn;
    private WaveFileWriter? writer;
    private string? currentPath;
    private float smoothedLevel;

    public WavAudioRecorder(int deviceNumber = 0)
    {
        tempDirectory = Path.Combine(Path.GetTempPath(), "CodexVoiceInput");
        Directory.CreateDirectory(tempDirectory);
        CleanupStaleRecordings(tempDirectory);
        this.deviceNumber = deviceNumber;
    }

    public string? CurrentPath => currentPath;
    public bool IsRecording => waveIn is not null;
    public event Action<float>? AudioLevelChanged;

    public Task StartAsync()
    {
        if (waveIn is not null)
        {
            throw new InvalidOperationException("Recording is already active.");
        }

        if (WaveIn.DeviceCount <= 0)
        {
            throw new InvalidOperationException("未找到麦克风输入设备");
        }

        if (deviceNumber < 0 || deviceNumber >= WaveIn.DeviceCount)
        {
            throw new InvalidOperationException($"麦克风设备编号无效：{deviceNumber}，可用设备数：{WaveIn.DeviceCount}");
        }

        currentPath = Path.Combine(tempDirectory, $"voice-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.wav");
        waveIn = new WaveInEvent
        {
            DeviceNumber = deviceNumber,
            WaveFormat = new WaveFormat(16_000, 16, 1),
            BufferMilliseconds = 50,
        };
        writer = new WaveFileWriter(currentPath, waveIn.WaveFormat);
        waveIn.DataAvailable += OnDataAvailable;
        waveIn.RecordingStopped += OnRecordingStopped;
        waveIn.StartRecording();
        return Task.CompletedTask;
    }

    public Task<string> StopAsync()
    {
        if (waveIn is null || currentPath is null)
        {
            throw new InvalidOperationException("Recording is not active.");
        }

        var activeWaveIn = waveIn;
        var outputPath = currentPath;
        var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        void Handler(object? sender, StoppedEventArgs args)
        {
            activeWaveIn.RecordingStopped -= Handler;
            if (args.Exception is not null)
            {
                completion.TrySetException(args.Exception);
            }
            else
            {
                completion.TrySetResult(outputPath);
            }
        }

        activeWaveIn.RecordingStopped += Handler;
        activeWaveIn.StopRecording();
        return completion.Task;
    }

    public static IReadOnlyList<string> ListDevices()
    {
        var devices = new List<string>();
        for (var i = 0; i < WaveIn.DeviceCount; i++)
        {
            var caps = WaveIn.GetCapabilities(i);
            devices.Add($"{i}: {caps.ProductName}");
        }

        return devices;
    }

    public static void CleanupStaleRecordings(string? directory = null)
    {
        var targetDirectory = directory ?? Path.Combine(Path.GetTempPath(), "CodexVoiceInput");
        if (!Directory.Exists(targetDirectory))
        {
            return;
        }

        var cutoff = DateTime.UtcNow.AddDays(-1);
        foreach (var path in Directory.EnumerateFiles(targetDirectory, "voice-*.wav"))
        {
            try
            {
                if (File.GetLastWriteTimeUtc(path) < cutoff)
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // A locked or inaccessible stale file can be retried on the next launch.
            }
        }
    }

    public void DeleteCurrentFile()
    {
        if (currentPath is { } path && File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void Dispose()
    {
        waveIn?.StopRecording();
        waveIn?.Dispose();
        writer?.Dispose();
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs args)
    {
        writer?.Write(args.Buffer, 0, args.BytesRecorded);

        double sum = 0;
        var samples = args.BytesRecorded / 2;
        for (var offset = 0; offset + 1 < args.BytesRecorded; offset += 2)
        {
            var sample = BinaryPrimitives.ReadInt16LittleEndian(args.Buffer.AsSpan(offset, 2));
            var normalized = sample / 32768f;
            sum += normalized * normalized;
        }

        if (samples > 0)
        {
            var rms = (float)Math.Sqrt(sum / samples);
            var responsiveLevel = Math.Clamp((rms - 0.004f) * 9f, 0f, 1f);
            smoothedLevel = Math.Max(responsiveLevel, smoothedLevel * 0.72f);
            AudioLevelChanged?.Invoke(smoothedLevel);
        }
    }

    private void OnRecordingStopped(object? sender, StoppedEventArgs args)
    {
        waveIn?.Dispose();
        writer?.Dispose();
        waveIn = null;
        writer = null;
        smoothedLevel = 0;
        AudioLevelChanged?.Invoke(0);
    }
}
