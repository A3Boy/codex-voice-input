$ErrorActionPreference = "Stop"

$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$window = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\MainWindow.xaml.cs")
$history = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\Services\RecognitionHistoryStore.cs")
$capsule = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\Services\LayeredCapsuleWindow.cs")
$settings = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\SettingsWindow.xaml")
$failures = [System.Collections.Generic.List[string]]::new()

if ($history -notmatch 'history\.json' -or $history -notmatch 'MaximumEntries = 200') {
    $failures.Add("recognition history must persist locally with a bounded entry count")
}
if ($window -notmatch 'historyStore\.Add' -or $window -notmatch 'Clipboard\.SetContent') {
    $failures.Add("completed recognition must enter history and support clipboard copy")
}
if ($capsule -notmatch 'ShowCopyConfirmation' -or $settings -notmatch 'HistoryList') {
    $failures.Add("result preview and settings UI must expose copy and history controls")
}
if ($window -notmatch 'finally[\s\S]*Recording file deleted after transcription') {
    $failures.Add("temporary audio must be deleted after every transcription attempt")
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Output "FAIL: $_" }
    exit 1
}

Write-Output "history-contract=pass"
