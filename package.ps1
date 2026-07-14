param(
    [switch] $SkipDesktopShortcut,
    [switch] $SkipLaunch
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$distRoot = Join-Path $root "dist"
$publishDir = Join-Path $distRoot "CodexVoiceInput"
$zipPath = Join-Path $distRoot "CodexVoiceInput-win-x64.zip"
$programsRoot = Join-Path $env:LOCALAPPDATA "Programs"
$installDir = Join-Path $programsRoot "CodexVoiceInput"
$buildOutput = Join-Path $root "src\VoiceInput.App\bin\x64\Release\net8.0-windows10.0.19041.0"

function Reset-Directory([string] $Path, [string] $AllowedRoot) {
    $fullPath = [IO.Path]::GetFullPath($Path)
    $fullRoot = [IO.Path]::GetFullPath($AllowedRoot).TrimEnd('\') + '\'
    if (-not $fullPath.StartsWith($fullRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to clear path outside ${AllowedRoot}: $fullPath"
    }
    if (Test-Path -LiteralPath $fullPath) {
        Remove-Item -LiteralPath $fullPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
}

Get-Process CodexVoiceInput -ErrorAction SilentlyContinue | Stop-Process
Reset-Directory -Path $publishDir -AllowedRoot $distRoot

& (Join-Path $root "build.ps1") -Configuration Release
if ($LASTEXITCODE -ne 0) {
    throw "Release build failed with exit code $LASTEXITCODE."
}

foreach ($requiredFile in @("CodexVoiceInput.exe", "App.xbf", "MainWindow.xbf", "SettingsWindow.xbf")) {
    if (-not (Test-Path -LiteralPath (Join-Path $buildOutput $requiredFile))) {
        throw "Release output is incomplete. Missing: $requiredFile"
    }
}
Get-ChildItem -LiteralPath $buildOutput | Where-Object { $_.Name -ne "win-x64" } | Copy-Item -Destination $publishDir -Recurse -Force

$publishedExe = Join-Path $publishDir "CodexVoiceInput.exe"
if (-not (Test-Path -LiteralPath $publishedExe)) {
    throw "Published executable not found: $publishedExe"
}

if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}
& tar.exe -a -c -f $zipPath -C $publishDir .
if ($LASTEXITCODE -ne 0) {
    throw "Portable ZIP creation failed with exit code $LASTEXITCODE."
}

Reset-Directory -Path $installDir -AllowedRoot $programsRoot
Copy-Item -Path (Join-Path $publishDir "*") -Destination $installDir -Recurse -Force
$installedExe = Join-Path $installDir "CodexVoiceInput.exe"

$logPath = Join-Path $env:LOCALAPPDATA "CodexVoiceInput\codex-voice-input.log"
$logBefore = if (Test-Path -LiteralPath $logPath) { [IO.File]::ReadAllText($logPath).Length } else { 0 }
$verificationProcess = Start-Process -FilePath $installedExe -ArgumentList "--open-settings" -PassThru
$verified = $false
try {
    for ($attempt = 0; $attempt -lt 20; $attempt++) {
        Start-Sleep -Milliseconds 500
        $verificationProcess.Refresh()
        if ($verificationProcess.HasExited) {
            break
        }
        if (Test-Path -LiteralPath $logPath) {
            $logContent = [IO.File]::ReadAllText($logPath)
            $newLog = if ($logContent.Length -gt $logBefore) { $logContent.Substring($logBefore) } else { "" }
            if ($newLog.Contains("Settings window opened.")) {
                $verified = $true
                break
            }
            if ($newLog.Contains("Application startup failed") -or $newLog.Contains("Settings window failed to open")) {
                break
            }
        }
    }
}
finally {
    $verificationProcess.Refresh()
    if (-not $verificationProcess.HasExited) {
        Stop-Process -Id $verificationProcess.Id
    }
}
if (-not $verified) {
    throw "Installed application failed its startup and settings verification."
}

if (-not $SkipDesktopShortcut) {
    $desktop = [Environment]::GetFolderPath([Environment+SpecialFolder]::DesktopDirectory)
    $shortcutPath = Join-Path $desktop "Codex Voice Input.lnk"
    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($shortcutPath)
    $shortcut.TargetPath = $installedExe
    $shortcut.WorkingDirectory = $installDir
    $shortcut.IconLocation = "$installedExe,0"
    $shortcut.Description = "Unofficial Codex Desktop voice-to-text input"
    $shortcut.Save()
    Write-Host "Desktop shortcut: $shortcutPath"
}

Write-Host "Installed application: $installedExe"
Write-Host "Portable package: $zipPath"

if (-not $SkipLaunch) {
    Start-Process -FilePath $installedExe
}
