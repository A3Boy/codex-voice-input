$ErrorActionPreference = "Stop"

$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$injector = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\Services\Win32TextInjector.cs")
$window = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\MainWindow.xaml.cs")
$capsule = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\Services\LayeredCapsuleWindow.cs")
$failures = [System.Collections.Generic.List[string]]::new()

if ($injector -notmatch "LayoutKind\.Explicit, Size\s*=\s*32") {
    $failures.Add("x64 INPUT union must be 32 bytes so SendInput receives cbSize=40")
}
if ($injector -notmatch "GetLastPInvokeError") {
    $failures.Add("SendInput failures must preserve the native Win32 error")
}
if ($capsule -notmatch "snapshot\.PreviewText") {
    $failures.Add("ready state must render the recognized PreviewText")
}
if ($window -notmatch "catch[\s\S]{0,240}capsuleWindow\.Show\(\)") {
    $failures.Add("injection failure must restore the capsule window")
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Output "FAIL: $_" }
    exit 1
}

Write-Output "ready-injection-contract=pass"
