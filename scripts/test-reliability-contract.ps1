$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$app = Get-Content (Join-Path $root "src\VoiceInput.App\App.xaml.cs") -Raw
$hotkey = Get-Content (Join-Path $root "src\VoiceInput.App\Services\GlobalHotkey.cs") -Raw
$engine = Get-Content (Join-Path $root "src\VoiceInput.Core\Recognition\CodexAsrRecognitionEngine.cs") -Raw
$recorder = Get-Content (Join-Path $root "src\VoiceInput.App\Services\WavAudioRecorder.cs") -Raw
$diagnostics = Get-Content (Join-Path $root "src\VoiceInput.App\Services\AppDiagnostics.cs") -Raw
$failures = [System.Collections.Generic.List[string]]::new()

if ($app -notmatch 'Mutex' -or $app -notmatch 'SingleInstanceMutexName') {
    $failures.Add("application must enforce a named-mutex single instance")
}
if ($hotkey -notmatch 'ModNoRepeat = 0x4000' -or $hotkey -notmatch 'Modifiers \| ModNoRepeat') {
    $failures.Add("global hotkey must use MOD_NOREPEAT")
}
if ($engine -notmatch 'HttpStatusCode\.Unauthorized or HttpStatusCode\.Forbidden' -or $engine -notmatch 'HttpStatusCode\.TooManyRequests') {
    $failures.Add("HTTP status codes must map to stable user-facing errors")
}
if ($recorder -notmatch 'AddDays\(-1\)' -or $recorder -notmatch 'voice-\*\.wav') {
    $failures.Add("stale temporary WAV files must be cleaned at startup")
}
if ($diagnostics -notmatch 'MaxLogBytes = 5 \* 1024 \* 1024' -or $diagnostics -notmatch 'File\.WriteAllText') {
    $failures.Add("diagnostic log must be bounded")
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "reliability-contract=pass"
