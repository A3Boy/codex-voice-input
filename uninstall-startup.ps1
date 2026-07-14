$ErrorActionPreference = "Stop"

$runKey = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
Remove-ItemProperty -Path $runKey -Name "CodexVoiceInput" -ErrorAction SilentlyContinue
Write-Host "Codex Voice Input startup entry removed."
