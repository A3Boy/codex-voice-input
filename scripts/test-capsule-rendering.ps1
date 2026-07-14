$ErrorActionPreference = "Stop"

$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$source = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\Services\LayeredCapsuleWindow.cs")

if ($source -notmatch 'DrawDockedCapsule' -or $source -notmatch 'TryDockAtEdge' -or $source -notmatch 'MonitorFromWindow') {
    throw "Capsule edge docking contract is missing."
}
$recorder = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\Services\WavAudioRecorder.cs")
$manifest = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\app.manifest")
$failures = [System.Collections.Generic.List[string]]::new()

if ($manifest -notmatch "PerMonitorV2") {
    $failures.Add("app manifest must declare PerMonitorV2 DPI awareness")
}
if ($source -notmatch "Cursor\s*=\s*LoadCursor") {
    $failures.Add("capsule window class must own an arrow cursor")
}
if ($source -notmatch "ScaleTransform\(dpiScale") {
    $failures.Add("layered bitmap drawing must scale logical coordinates for the monitor DPI")
}
if ($source -notmatch "TextRenderingHint\.AntiAliasGridFit") {
    $failures.Add("layered text must use deterministic antialiased glyph rendering")
}
if ($source -notmatch "Color\.FromArgb\(246,\s*255,\s*255,\s*255\)") {
    $failures.Add("capsule material must retain a translucent glass layer")
}
if ($source -notmatch "waveformPhase" -or $source -notmatch "UpdateAudioLevel") {
    $failures.Add("recording waveform must animate from live audio levels")
}
if ($source -notmatch "VoiceActivityThreshold" -or $source -notmatch "DrawRecordingBaseline") {
    $failures.Add("silent recording must remain a straight baseline until voice activity is detected")
}
if ($source -notmatch "AdvanceLoadingAnimation" -or $source -notmatch "loadingAngle") {
    $failures.Add("transcribing state must animate its loading ring frame by frame")
}
if ($source -notmatch "shouldExpandPreview" -or $source -notmatch "next\.PreviewText") {
    $failures.Add("recognized text preview must expand automatically when transcription completes")
}
if ($recorder -notmatch "AudioLevelChanged" -or $recorder -notmatch "ReadInt16LittleEndian") {
    $failures.Add("audio recorder must publish levels derived from microphone PCM samples")
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Output "FAIL: $_" }
    exit 1
}

Write-Output "capsule-rendering-contract=pass"
