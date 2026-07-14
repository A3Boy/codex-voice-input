using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Runtime.InteropServices;
using VoiceInput.App.Services;
using VoiceInput.Core.Capsule;
using VoiceInput.Core.Recognition;
using VoiceInput.Protocol;
using WinRT.Interop;

namespace VoiceInput.App;

public sealed partial class MainWindow : Window, IDisposable
{
    private readonly AppWindow appWindow;
    private readonly nint hwnd;
    private readonly AppConfig config;
    private WavAudioRecorder recorder;
    private int activeAudioDeviceNumber;
    private readonly CapsuleController controller;
    private readonly DispatcherTimer recordingTimer = new();
    private readonly DispatcherTimer loadingAnimationTimer = new();
    private readonly LayeredCapsuleWindow capsuleWindow;
    private GlobalHotkey? hotkey;
    private HotkeyDefinition? activeHotkeyDefinition;
    private readonly Win32TrayIcon trayIcon;
    private readonly RecognitionHistoryStore historyStore;
    private SettingsWindow? settingsWindow;
    private CapsuleSnapshot currentSnapshot = new(CapsuleState.Idle, "点击开始语音输入");
    private string readyText = string.Empty;
    private bool busy;
    private bool cancelRequested;
    private bool cancelInProgress;
    private DateTimeOffset recordingStartedAt;

    public MainWindow()
    {
        AppDiagnostics.Info("MainWindow starting.");
        InitializeComponent();
        config = AppConfig.Load();
        if (!HotkeyDefinition.TryParse(config.Hotkey, out var configuredHotkey, out var hotkeyConfigError))
        {
            AppDiagnostics.Info($"Invalid saved hotkey '{config.Hotkey}' reset to {HotkeyDefinition.Default.Display}: {hotkeyConfigError}");
            config.Hotkey = HotkeyDefinition.Default.Display;
            config.Save();
            configuredHotkey = HotkeyDefinition.Default;
        }

        hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        appWindow = AppWindow.GetFromWindowId(windowId);

        ConfigureWindow(hwnd);
        AppDiagnostics.Info("Audio devices: " + string.Join(" | ", WavAudioRecorder.ListDevices()));
        historyStore = new RecognitionHistoryStore();
        recorder = new WavAudioRecorder(config.AudioDeviceNumber);
        activeAudioDeviceNumber = config.AudioDeviceNumber;
        controller = new CapsuleController(CreateRecognitionEngine(config));
        controller.SnapshotChanged += (_, snapshot) => DispatcherQueue.TryEnqueue(() => ApplySnapshot(snapshot));
        recordingTimer.Interval = TimeSpan.FromMilliseconds(250);
        recordingTimer.Tick += (_, _) => UpdateRecordingTimer();
        capsuleWindow = new LayeredCapsuleWindow(
            () => DispatcherQueue.TryEnqueue(() => _ = ToggleAsync()),
            () => DispatcherQueue.TryEnqueue(ShowSettingsWindow),
            () => DispatcherQueue.TryEnqueue(() => _ = CancelOrHideAsync()),
            text => DispatcherQueue.TryEnqueue(() => CopyRecognitionText(text)));
        capsuleWindow.SetDarkMode(config.DarkMode);
        loadingAnimationTimer.Interval = TimeSpan.FromMilliseconds(50);
        loadingAnimationTimer.Tick += (_, _) => capsuleWindow.AdvanceLoadingAnimation();
        recorder.AudioLevelChanged += OnAudioLevelChanged;
        trayIcon = new Win32TrayIcon(DispatcherQueue, ShowCapsule, ShowSettingsWindow, OpenLogFile, ExitApplication);
        TryStartTrayIcon();

        if (Environment.GetEnvironmentVariable("VOICEINPUT_DISABLE_HOTKEY") != "1")
        {
            hotkey = CreateGlobalHotkey(configuredHotkey);
            if (TryRegisterHotkey())
            {
                activeHotkeyDefinition = configuredHotkey;
            }
        }

        ApplySnapshot(controller.Snapshot);
        capsuleWindow.Show();
        AppDiagnostics.Info("MainWindow started.");
    }

    private void ConfigureWindow(nint hwnd)
    {
        appWindow.Resize(new Windows.Graphics.SizeInt32(240, 44));
        MoveToDefaultPosition();
        appWindow.Title = "Codex Voice Input";

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(false, false);
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
        }

        var extendedStyle = GetWindowLong(hwnd, GwlExStyle);
        SetWindowLong(hwnd, GwlExStyle, extendedStyle | WsExToolWindow | WsExNoActivate);
        ApplyRoundedWindowRegion(hwnd, 240, 44, 22);
    }

    private void ApplySnapshot(CapsuleSnapshot snapshot)
    {
        var previousSnapshot = currentSnapshot;
        currentSnapshot = snapshot;
        if (snapshot.PreviewText is { Length: > 0 })
        {
            readyText = snapshot.PreviewText;
            if (snapshot.State == CapsuleState.Ready
                && (previousSnapshot.State != CapsuleState.Ready
                    || !string.Equals(previousSnapshot.PreviewText, snapshot.PreviewText, StringComparison.Ordinal)))
            {
                historyStore.Add(snapshot.PreviewText);
            }
        }

        StatusText.Text = snapshot.Message;
        trayIcon.SetTooltip($"Codex Voice Input - {snapshot.Message}");
        capsuleWindow.Update(snapshot);
        Waveform.Visibility = snapshot.State == CapsuleState.Recording ? Visibility.Visible : Visibility.Collapsed;
        StatusText.Visibility = snapshot.State == CapsuleState.Recording ? Visibility.Collapsed : Visibility.Visible;
        if (snapshot.State != CapsuleState.Recording)
        {
            recordingTimer.Stop();
            TimerText.Text = "00:00";
        }
        if (snapshot.State == CapsuleState.Transcribing)
        {
            loadingAnimationTimer.Start();
        }
        else
        {
            loadingAnimationTimer.Stop();
        }

        switch (snapshot.State)
        {
            case CapsuleState.Recording:
                MainIcon.Glyph = "\uE720";
                Capsule.BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(0x88, 0x2D, 0x84, 0xFF));
                Capsule.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFA, 0xFC, 0xFF));
                Root.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFA, 0xFC, 0xFF));
                MainButton.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0x24, 0x7C, 0xFF));
                MainIcon.Foreground = new SolidColorBrush(Colors.White);
                break;
            case CapsuleState.Transcribing:
                MainIcon.Glyph = "\uE9D9";
                Capsule.BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(0x66, 0x2D, 0x84, 0xFF));
                Capsule.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFA, 0xFC, 0xFF));
                Root.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFA, 0xFC, 0xFF));
                MainButton.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xF1, 0xF7, 0xFF));
                MainIcon.Foreground = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0x24, 0x7C, 0xFF));
                break;
            case CapsuleState.Ready:
                Capsule.BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(0x92, 0x46, 0xDB, 0x9D));
                Capsule.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xF4, 0xFF, 0xFA));
                Root.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xF4, 0xFF, 0xFA));
                MainButton.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0x26, 0xC9, 0x81));
                MainIcon.Glyph = "\uE73E";
                MainIcon.Foreground = new SolidColorBrush(Colors.White);
                break;
            case CapsuleState.Error:
                Capsule.BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(0x88, 0xFF, 0xB0, 0x20));
                Capsule.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFA, 0xFC, 0xFF));
                Root.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFA, 0xFC, 0xFF));
                MainButton.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFF, 0xF4, 0xD8));
                MainIcon.Foreground = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0x9A, 0x62, 0x00));
                break;
            default:
                Capsule.BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(0x3D, 0x84, 0x92, 0xA6));
                Capsule.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFA, 0xFC, 0xFF));
                Root.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xFA, 0xFC, 0xFF));
                MainButton.Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0xF8, 0xFA, 0xFD));
                MainIcon.Glyph = "\uE720";
                MainIcon.Foreground = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0x16, 0x22, 0x30));
                break;
        }
    }

    private void MainButton_Click(object sender, RoutedEventArgs e)
    {
        _ = ToggleAsync();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        ShowSettingsWindow();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        HideCapsule();
    }

    private async Task ToggleAsync()
    {
        if (busy) return;

        try
        {
            busy = true;
            switch (currentSnapshot.State)
            {
                case CapsuleState.Idle:
                case CapsuleState.Sleeping:
                case CapsuleState.Error:
                    await StartRecordingAsync();
                    break;
                case CapsuleState.Recording:
                    await StopAndTranscribeAsync();
                    break;
                case CapsuleState.Ready:
                    await InjectReadyTextAsync();
                    break;
            }
        }
        catch (Exception error)
        {
            AppDiagnostics.Error("Toggle failed", error);
            capsuleWindow.Show();
            ApplySnapshot(new CapsuleSnapshot(CapsuleState.Error, error.Message));
        }
        finally
        {
            busy = false;
        }
    }

    private async Task StartRecordingAsync()
    {
        AppDiagnostics.Info("Recording start requested.");
        cancelRequested = false;
        capsuleWindow.Show();
        await recorder.StartAsync();
        AppDiagnostics.Info("Recording started.");
        recordingStartedAt = DateTimeOffset.Now;
        recordingTimer.Start();
        controller.StartRecording();
    }

    private async Task StopAndTranscribeAsync()
    {
        AppDiagnostics.Info("Recording stop requested.");
        var audioPath = await recorder.StopAsync();
        var length = File.Exists(audioPath) ? new FileInfo(audioPath).Length : 0;
        AppDiagnostics.Info($"Recording stopped. path={audioPath} bytes={length}");
        if (cancelRequested)
        {
            recorder.DeleteCurrentFile();
            readyText = string.Empty;
            cancelRequested = false;
            controller.Reset();
            AppDiagnostics.Info("Recording cancelled before transcription.");
            return;
        }

        try
        {
            await controller.StopAndTranscribeAsync(audioPath);
            AppDiagnostics.Info($"Transcription state={controller.Snapshot.State} message={controller.Snapshot.Message}");
        }
        finally
        {
            recorder.DeleteCurrentFile();
            AppDiagnostics.Info($"Recording file deleted after transcription: {audioPath}");
            cancelRequested = false;
        }
    }

    private async Task CancelOrHideAsync()
    {
        if (cancelInProgress)
        {
            return;
        }

        switch (currentSnapshot.State)
        {
            case CapsuleState.Recording:
                cancelRequested = true;
                readyText = string.Empty;
                recordingTimer.Stop();
                if (busy)
                {
                    controller.Reset();
                    return;
                }

                cancelInProgress = true;
                try
                {
                    if (recorder.IsRecording)
                    {
                        await recorder.StopAsync();
                    }
                    recorder.DeleteCurrentFile();
                    controller.Reset();
                    AppDiagnostics.Info("Recording cancelled by user.");
                }
                catch (Exception error)
                {
                    AppDiagnostics.Error("Cancel recording failed", error);
                    capsuleWindow.Show();
                    ApplySnapshot(new CapsuleSnapshot(CapsuleState.Error, error.Message));
                }
                finally
                {
                    cancelRequested = false;
                    cancelInProgress = false;
                }
                break;
            case CapsuleState.Transcribing:
                cancelRequested = true;
                readyText = string.Empty;
                controller.CancelTranscription();
                controller.Reset();
                AppDiagnostics.Info("Transcription cancellation requested by user.");
                break;
            case CapsuleState.Ready:
            case CapsuleState.Error:
                readyText = string.Empty;
                controller.Reset();
                break;
            default:
                HideCapsule();
                break;
        }
    }

    private async Task InjectReadyTextAsync()
    {
        if (string.IsNullOrWhiteSpace(readyText))
        {
            controller.Reset();
            return;
        }

        await Task.Delay(60);
        try
        {
            AppDiagnostics.Info($"Injecting text length={readyText.Length}.");
            Win32TextInjector.TypeText(readyText);
            AppDiagnostics.Info("Text injected.");
            readyText = string.Empty;
            controller.Reset();
            if (config.ShowAfterInput)
            {
                capsuleWindow.Show();
            }
            else
            {
                capsuleWindow.Hide();
            }
        }
        catch
        {
            capsuleWindow.Show();
            throw;
        }
    }

    private static IRecognitionEngine CreateRecognitionEngine(AppConfig config)
    {
        var repoRoot = FindRepoRoot(AppContext.BaseDirectory);
        var authFile = !string.IsNullOrWhiteSpace(config.AuthFile)
            ? config.AuthFile
            : ResolveAuthFile(repoRoot);
        AppDiagnostics.Info($"Recognition engine=Codex Desktop ASR (.NET) auth={(authFile is null ? "<none>" : authFile)}");
        return new CodexAsrRecognitionEngine(authFile);
    }

    private static string? ResolveAuthFile(string? repoRoot)
    {
        var candidates = new[]
        {
            Environment.GetEnvironmentVariable("CODEX_ASR_AUTH_FILE"),
            Environment.GetEnvironmentVariable("CODEX_HOME") is { Length: > 0 } codexHome
                ? Path.Combine(codexHome, "auth.json")
                : null,
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) is { Length: > 0 } userProfile
                ? Path.Combine(userProfile, ".codex", "auth.json")
                : null,
            repoRoot is null ? null : Path.Combine(repoRoot, ".codex", "auth.json"),
        };

        return candidates.FirstOrDefault(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path));
    }

    private static string? FindRepoRoot(string start)
    {
        var directory = new DirectoryInfo(start);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private GlobalHotkey CreateGlobalHotkey(HotkeyDefinition definition)
    {
        var result = new GlobalHotkey(this, definition);
        result.Pressed += (_, _) => _ = ToggleAsync();
        return result;
    }

    private bool TryRegisterHotkey()
    {
        try
        {
            hotkey?.Register();
            AppDiagnostics.Info("Global hotkey registered.");
            return true;
        }
        catch (Exception error)
        {
            AppDiagnostics.Error("Global hotkey registration failed", error);
            ApplySnapshot(new CapsuleSnapshot(CapsuleState.Error, error.Message));
            return false;
        }
    }

    private void TryStartTrayIcon()
    {
        try
        {
            trayIcon.Start();
            AppDiagnostics.Info("Tray icon started.");
        }
        catch (Exception error)
        {
            AppDiagnostics.Error("Tray icon failed", error);
            ApplySnapshot(new CapsuleSnapshot(CapsuleState.Error, error.Message));
        }
    }

    public void Dispose()
    {
        hotkey?.Dispose();
        recordingTimer.Stop();
        loadingAnimationTimer.Stop();
        recorder.AudioLevelChanged -= OnAudioLevelChanged;
        settingsWindow?.Close();
        trayIcon.Dispose();
        capsuleWindow.Dispose();
        recorder.Dispose();
    }

    public void HideHostWindow()
    {
        ShowWindow(hwnd, SwHide);
    }

    private void ShowCapsule()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            capsuleWindow.Show();
        });
    }

    private void HideCapsule()
    {
        capsuleWindow.Hide();
    }

    private void ExitApplication()
    {
        Dispose();
        App.Current.Exit();
    }

    private void ShowSettingsWindow()
    {
        if (settingsWindow is not null)
        {
            settingsWindow.Activate();
            return;
        }

        try
        {
            settingsWindow = new SettingsWindow(config, ApplyRuntimeSettings, historyStore);
            settingsWindow.Closed += (_, _) => settingsWindow = null;
            settingsWindow.Activate();
            AppDiagnostics.Info("Settings window opened.");
        }
        catch (Exception error)
        {
            settingsWindow = null;
            AppDiagnostics.Error("Settings window failed to open", error);
        }
    }

    internal void OpenSettingsForDiagnostics() => ShowSettingsWindow();

    private string? ApplyRuntimeSettings(AppConfig updated)
    {
        capsuleWindow.SetDarkMode(updated.DarkMode);
        var requestedAudioDevice = updated.AudioDeviceNumber;
        var audioDeviceChanged = requestedAudioDevice != activeAudioDeviceNumber;
        if (audioDeviceChanged)
        {
            if (recorder.IsRecording || currentSnapshot.State == CapsuleState.Transcribing)
            {
                return "请先结束当前录音或转写，再切换麦克风。";
            }
            var deviceCount = WavAudioRecorder.ListDevices().Count;
            if (requestedAudioDevice < 0 || requestedAudioDevice >= deviceCount)
            {
                return "所选麦克风当前不可用，请重新选择。";
            }
        }

        var requestedHotkey = HotkeyDefinition.Parse(updated.Hotkey);
        var hotkeyChanged = Environment.GetEnvironmentVariable("VOICEINPUT_DISABLE_HOTKEY") != "1"
            && (activeHotkeyDefinition is null
                || activeHotkeyDefinition.Modifiers != requestedHotkey.Modifiers
                || activeHotkeyDefinition.VirtualKey != requestedHotkey.VirtualKey);
        if (hotkeyChanged)
        {
            var hotkeyError = ChangeGlobalHotkey(requestedHotkey);
            if (hotkeyError is not null)
            {
                return hotkeyError;
            }
        }

        if (audioDeviceChanged)
        {
            var replacement = new WavAudioRecorder(requestedAudioDevice);
            replacement.AudioLevelChanged += OnAudioLevelChanged;
            var previousRecorder = recorder;
            recorder = replacement;
            activeAudioDeviceNumber = requestedAudioDevice;
            previousRecorder.AudioLevelChanged -= OnAudioLevelChanged;
            previousRecorder.Dispose();
            AppDiagnostics.Info($"Audio input device changed to {requestedAudioDevice}.");
        }

        AppDiagnostics.Info($"Runtime settings applied. darkMode={updated.DarkMode}");
        return null;
    }

    private string? ChangeGlobalHotkey(HotkeyDefinition requestedHotkey)
    {
        var previousDefinition = activeHotkeyDefinition;
        hotkey?.Dispose();
        hotkey = null;
        activeHotkeyDefinition = null;

        var replacement = CreateGlobalHotkey(requestedHotkey);
        try
        {
            replacement.Register();
            hotkey = replacement;
            activeHotkeyDefinition = requestedHotkey;
            AppDiagnostics.Info($"Global hotkey changed to {requestedHotkey.Display}.");
            return null;
        }
        catch (Exception error)
        {
            replacement.Dispose();
            AppDiagnostics.Error("Global hotkey change failed", error);
            if (previousDefinition is not null)
            {
                hotkey = CreateGlobalHotkey(previousDefinition);
                if (TryRegisterHotkey())
                {
                    activeHotkeyDefinition = previousDefinition;
                }
            }
            return $"快捷键 {requestedHotkey.Display} 注册失败，可能已被其他程序占用。已恢复原快捷键。";
        }
    }

    private void CopyRecognitionText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (!ClipboardService.TrySetText(text, out var error))
        {
            AppDiagnostics.Info($"Recognition text copy failed: {error}");
            capsuleWindow.ShowCopyFailure();
            return;
        }
        capsuleWindow.ShowCopyConfirmation();
        AppDiagnostics.Info($"Recognition text copied. length={text.Length}");
    }

    private void OpenLogFile()
    {
        OpenPath(AppDiagnostics.FilePath);
    }

    private static void OpenPath(string path)
    {
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, string.Empty);
        }

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true,
        });
    }

    private static void ApplyRoundedWindowRegion(nint hwnd, int width, int height, int radius)
    {
        var region = CreateRoundRectRgn(0, 0, width + 1, height + 1, radius * 2, radius * 2);
        SetWindowRgn(hwnd, region, true);
    }

    private void MoveToDefaultPosition()
    {
        var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);
        var workArea = displayArea.WorkArea;
        appWindow.Move(new Windows.Graphics.PointInt32(
            workArea.X + workArea.Width - 240 - 32,
            workArea.Y + workArea.Height - 44 - 96));
    }

    private void UpdateRecordingTimer()
    {
        var elapsed = DateTimeOffset.Now - recordingStartedAt;
        var text = $"{(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00}";
        TimerText.Text = text;
        capsuleWindow.UpdateTimer(text);
    }

    private void OnAudioLevelChanged(float level)
    {
        DispatcherQueue.TryEnqueue(() => capsuleWindow.UpdateAudioLevel(level));
    }

    private const int GwlExStyle = -20;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;
    private const int SwHide = 0;
    private const int SwShowNoActivate = 4;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("gdi32.dll")]
    private static extern nint CreateRoundRectRgn(int left, int top, int right, int bottom, int widthEllipse, int heightEllipse);

    [DllImport("user32.dll")]
    private static extern int SetWindowRgn(nint hWnd, nint hRgn, bool redraw);
}
