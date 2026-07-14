param(
  [ValidateSet("Debug", "Release")]
  [string] $Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$exe = Join-Path $root "src\VoiceInput.App\bin\x64\$Configuration\net8.0-windows10.0.19041.0\CodexVoiceInput.exe"

if (-not (Test-Path $exe)) {
  & (Join-Path $root "build.ps1") -Configuration $Configuration
}

Start-Process -FilePath $exe
