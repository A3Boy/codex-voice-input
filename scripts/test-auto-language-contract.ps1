$ErrorActionPreference = "Stop"

$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$codex = Get-Content -Raw (Join-Path $root "src\VoiceInput.Core\Recognition\CodexAsrRecognitionEngine.cs")
$controller = Get-Content -Raw (Join-Path $root "src\VoiceInput.Core\Capsule\CapsuleController.cs")
$mainWindow = Get-Content -Raw (Join-Path $root "src\VoiceInput.App\MainWindow.xaml.cs")
$failures = [System.Collections.Generic.List[string]]::new()

if ($codex -notmatch "HasExplicitLanguage" -or $codex -notmatch 'Equals\("auto"') {
    $failures.Add("Codex automatic language detection must omit the upstream language field")
}
if ($controller -notmatch "new RecognitionOptions\(\)" -or $controller -match "SetLanguage") {
    $failures.Add("voice input must always rely on Codex automatic language detection")
}
if ((Test-Path (Join-Path $root "src\VoiceInput.Core\Recognition\OpenAiTranscriptionEngine.cs")) -or $mainWindow -match "OPENAI_API_KEY|OpenAiTranscriptionEngine") {
    $failures.Add("application must contain only the Codex reverse-engineered transcription path")
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Output "FAIL: $_" }
    exit 1
}

Write-Output "auto-language-contract=pass"
