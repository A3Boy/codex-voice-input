$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$installDir = Join-Path $env:LOCALAPPDATA "Programs\CodexVoiceInput"
$exe = Join-Path $installDir "CodexVoiceInput.exe"
if (-not (Test-Path -LiteralPath $exe)) {
    & (Join-Path $root "package.ps1") -SkipLaunch
}
if (-not (Test-Path -LiteralPath $exe)) {
    throw "CodexVoiceInput.exe was not installed at $exe"
}

$runKey = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
New-Item -Path $runKey -Force | Out-Null
Set-ItemProperty -Path $runKey -Name "CodexVoiceInput" -Value "`"$exe`""
Write-Host "Codex Voice Input will start with Windows: $exe"
