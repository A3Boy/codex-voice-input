using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using VoiceInput.App.Services;
using WinRT.Interop;

namespace VoiceInput.App;

public sealed partial class SettingsWindow : Window
{
    private readonly AppConfig config;
    private readonly Func<AppConfig, string?> settingsApplied;
    private readonly RecognitionHistoryStore historyStore;
    private readonly AppWindow appWindow;

    public SettingsWindow(
        AppConfig config,
        Func<AppConfig, string?> settingsApplied,
        RecognitionHistoryStore historyStore)
    {
        this.config = config;
        this.settingsApplied = settingsApplied;
        this.historyStore = historyStore;
        InitializeComponent();

        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        appWindow = AppWindow.GetFromWindowId(windowId);
        ConfigureWindow();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBarDragRegion);
        TryApplyBackdrop();
        LoadValues();
        ApplyTheme();
        ReloadHistory();
        historyStore.Changed += HistoryStore_Changed;
        Closed += (_, _) => historyStore.Changed -= HistoryStore_Changed;
    }

    private void ConfigureWindow()
    {
        appWindow.Resize(new Windows.Graphics.SizeInt32(390, 450));
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
        }
    }

    private void TryApplyBackdrop()
    {
        try
        {
            SystemBackdrop = new DesktopAcrylicBackdrop();
        }
        catch
        {
            // The translucent gradient remains usable when acrylic is unavailable.
        }
    }

    private void LoadValues()
    {
        MicrophoneBox.ItemsSource = WavAudioRecorder.ListDevices();
        MicrophoneBox.SelectedIndex = config.AudioDeviceNumber >= 0
            && config.AudioDeviceNumber < MicrophoneBox.Items.Count
                ? config.AudioDeviceNumber
                : 0;
        HotkeyBox.Text = config.Hotkey;
        ShowAfterInputToggle.IsOn = config.ShowAfterInput;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!HotkeyDefinition.TryParse(HotkeyBox.Text, out var hotkeyDefinition, out var validationError))
        {
            StatusText.Text = validationError;
            return;
        }

        var previousHotkey = config.Hotkey;
        var previousAudioDevice = config.AudioDeviceNumber;
        config.AudioDeviceNumber = Math.Max(0, MicrophoneBox.SelectedIndex);
        config.Hotkey = hotkeyDefinition.Display;
        config.ShowAfterInput = ShowAfterInputToggle.IsOn;
        var applyError = settingsApplied(config);
        if (applyError is not null)
        {
            config.Hotkey = previousHotkey;
            config.AudioDeviceNumber = previousAudioDevice;
            HotkeyBox.Text = previousHotkey;
            MicrophoneBox.SelectedIndex = previousAudioDevice;
            StatusText.Text = applyError;
            return;
        }

        config.Save();
        HotkeyBox.Text = hotkeyDefinition.Display;
        StatusText.Text = "设置已保存，快捷键和麦克风已立即生效。";
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        config.DarkMode = !config.DarkMode;
        config.Save();
        ApplyTheme();
        _ = settingsApplied(config);
    }

    private void HistoryStore_Changed(object? sender, EventArgs e)
    {
        DispatcherQueue.TryEnqueue(ReloadHistory);
    }

    private void ReloadHistory()
    {
        var items = historyStore.Snapshot()
            .Select(entry => new HistoryViewItem(
                entry.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                entry.Text))
            .ToArray();
        HistoryList.ItemsSource = items;
        EmptyHistoryText.Visibility = items.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void CopyHistory_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string text } || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (!ClipboardService.TrySetText(text, out var error))
        {
            StatusText.Text = $"复制失败：{error}";
        }
    }

    private void ClearHistory_Click(object sender, RoutedEventArgs e)
    {
        historyStore.Clear();
    }

    private void ApplyTheme()
    {
        Root.RequestedTheme = config.DarkMode ? ElementTheme.Dark : ElementTheme.Light;
        ThemeIcon.Glyph = config.DarkMode ? "\uE706" : "\uE708";
        var background = new LinearGradientBrush
        {
            StartPoint = new Windows.Foundation.Point(0, 0),
            EndPoint = new Windows.Foundation.Point(1, 1),
        };
        if (config.DarkMode)
        {
            background.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(238, 32, 43, 57), Offset = 0 });
            background.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(242, 18, 27, 38), Offset = 1 });
            appWindow.TitleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(255, 225, 235, 248);
        }
        else
        {
            background.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(242, 255, 255, 255), Offset = 0 });
            background.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(238, 241, 246, 252), Offset = 1 });
            appWindow.TitleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(255, 34, 48, 66);
        }
        Root.Background = background;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public sealed record HistoryViewItem(string DisplayTime, string Text);
}
