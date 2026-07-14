$ErrorActionPreference = "Stop"

$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$window = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\MainWindow.xaml.cs")
$settingsXaml = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\SettingsWindow.xaml")
$settingsCode = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\SettingsWindow.xaml.cs")
$appXaml = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\App.xaml")
$capsule = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\Services\LayeredCapsuleWindow.cs")
$failures = [System.Collections.Generic.List[string]]::new()

if ($appXaml -notmatch "XamlControlsResources") {
    $failures.Add("application must merge WinUI control resources before opening settings")
}

if ($window -notmatch "ShowSettingsWindow" -or $window -match "DispatcherQueue\.TryEnqueue\(OpenConfigFile\)") {
    $failures.Add("settings actions must open an in-app settings window")
}
if ($settingsXaml -notmatch "MicrophoneBox" -or $settingsXaml -notmatch "HotkeyBox" -or $settingsXaml -notmatch "ThemeButton") {
    $failures.Add("settings UI must expose microphone, hotkey, and compact theme controls")
}
if ($settingsXaml -match "LanguageBox|ServiceBox|OpenAI") {
    $failures.Add("settings UI must not expose language or alternate transcription providers")
}
if ($settingsCode -notmatch "config\.Save\(\)" -or $settingsCode -notmatch "DesktopAcrylicBackdrop") {
    $failures.Add("settings UI must persist changes and retain the glass material")
}
if ($settingsCode -notmatch "HotkeyDefinition\.TryParse" -or $settingsCode -notmatch "快捷键和麦克风已立即生效") {
    $failures.Add("settings UI must validate and immediately apply global hotkeys")
}
if ($window -notmatch "Global hotkey changed to" -or $window -notmatch "previousDefinition") {
    $failures.Add("runtime settings must replace the hotkey and restore the previous binding on failure")
}
if ($window -notmatch "Audio input device changed to" -or $window -notmatch "activeAudioDeviceNumber") {
    $failures.Add("runtime settings must replace the active microphone immediately")
}
if ($capsule -notmatch "DefaultCapsuleWidth = 240" -or $capsule -notmatch "DrawPreviewPanel" -or $capsule -notmatch "DrawChevron") {
    $failures.Add("capsule must preserve the 240px reference layout and expose full recognized text through the ready-state preview")
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Output "FAIL: $_" }
    exit 1
}

Write-Output "settings-ui-contract=pass"
