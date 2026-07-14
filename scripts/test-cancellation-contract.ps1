$ErrorActionPreference = "Stop"

$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$controller = Get-Content -Raw (Join-Path $root "src\VoiceInput.Core\Capsule\CapsuleController.cs")
$window = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\MainWindow.xaml.cs")
$failures = [System.Collections.Generic.List[string]]::new()

if ($controller -notmatch "CancelTranscription" -or $controller -notmatch "OperationCanceledException") {
    $failures.Add("controller must cancel an active recognition request without publishing an error")
}
if ($window -notmatch "CancelOrHideAsync" -or $window -notmatch "Recording cancelled by user") {
    $failures.Add("capsule close action must cancel active recording instead of hiding the window")
}
if ($window -notmatch "CapsuleState\.Transcribing[\s\S]*CancelTranscription") {
    $failures.Add("transcribing state must propagate cancellation to the recognition engine")
}
if ($window -notmatch "CapsuleState\.Ready[\s\S]*readyText = string\.Empty") {
    $failures.Add("ready-state cancellation must discard recognized text")
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Output "FAIL: $_" }
    exit 1
}

Write-Output "cancellation-contract=pass"
