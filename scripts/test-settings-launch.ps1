param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$exe = Join-Path $root "src\VoiceInput.App\bin\x64\$Configuration\net8.0-windows10.0.19041.0\CodexVoiceInput.exe"
$log = Join-Path $env:LOCALAPPDATA "CodexVoiceInput\codex-voice-input.log"

if (-not (Test-Path -LiteralPath $exe)) {
    throw "Build the application before running the settings launch test."
}
if (Get-Process CodexVoiceInput -ErrorAction SilentlyContinue) {
    throw "Close CodexVoiceInput before running the settings launch test."
}

$before = if (Test-Path -LiteralPath $log) { [IO.File]::ReadAllText($log).Length } else { 0 }
$process = Start-Process -FilePath $exe -ArgumentList "--open-settings" -PassThru
try {
    Start-Sleep -Seconds 3
    $process.Refresh()
    $content = if (Test-Path -LiteralPath $log) { [IO.File]::ReadAllText($log) } else { "" }
    $newLog = if ($content.Length -gt $before) { $content.Substring($before) } else { "" }
    if ($process.HasExited) {
        throw "Application exited while opening settings."
    }
    if ($newLog -notmatch "Settings window opened\.") {
        throw "Settings window did not report a successful open.`n$newLog"
    }
    if ($newLog -match "Settings window failed to open") {
        throw "Settings window reported an exception.`n$newLog"
    }
    Write-Output "settings-launch=pass"
}
finally {
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id
    }
}
