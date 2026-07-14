param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"

$msbuild = if (Test-Path -LiteralPath $vswhere) {
    & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
} else {
    $null
}
if (-not $msbuild) {
    $command = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($command) {
        $msbuild = $command.Source
    }
}
if (-not $msbuild) {
    throw "MSBuild was not found. Install Visual Studio 2022 or Build Tools with .NET desktop and C++ workloads."
}

& $msbuild (Join-Path $root "CodexVoiceInput.sln") `
    /restore `
    /p:Configuration=$Configuration `
    /p:Platform=x64 `
    /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "Build failed with exit code $LASTEXITCODE."
}
