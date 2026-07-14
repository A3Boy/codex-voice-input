param(
    [string] $Version = "latest",
    [switch] $NoShortcut,
    [switch] $NoLaunch
)

$ErrorActionPreference = "Stop"
$repository = "A3Boy/codex-voice-input"
$api = if ($Version -eq "latest") {
    "https://api.github.com/repos/$repository/releases/latest"
} else {
    "https://api.github.com/repos/$repository/releases/tags/$Version"
}
$headers = @{ "User-Agent" = "CodexVoiceInput-Installer" }
$release = Invoke-RestMethod -Uri $api -Headers $headers
$zipAsset = $release.assets | Where-Object name -eq "CodexVoiceInput-win-x64.zip" | Select-Object -First 1
$sumAsset = $release.assets | Where-Object name -eq "SHA256SUMS.txt" | Select-Object -First 1
if (-not $zipAsset -or -not $sumAsset) {
    throw "Release assets are incomplete. Expected the Windows ZIP and SHA256SUMS.txt."
}

$tempRoot = Join-Path ([IO.Path]::GetTempPath()) ("CodexVoiceInput-install-" + [Guid]::NewGuid().ToString("N"))
$installRoot = Join-Path $env:LOCALAPPDATA "Programs"
$installDir = Join-Path $installRoot "CodexVoiceInput"
New-Item -ItemType Directory -Path $tempRoot | Out-Null
try {
    $zipPath = Join-Path $tempRoot $zipAsset.name
    $sumPath = Join-Path $tempRoot $sumAsset.name
    Invoke-WebRequest -Uri $zipAsset.browser_download_url -Headers $headers -OutFile $zipPath
    Invoke-WebRequest -Uri $sumAsset.browser_download_url -Headers $headers -OutFile $sumPath
    $expected = ((Get-Content -LiteralPath $sumPath | Select-Object -First 1) -split '\s+')[0].Trim().ToLowerInvariant()
    $actual = (Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($actual -ne $expected) {
        throw "SHA-256 verification failed. Expected $expected, received $actual."
    }

    Get-Process CodexVoiceInput -ErrorAction SilentlyContinue | Stop-Process
    $fullInstall = [IO.Path]::GetFullPath($installDir)
    $fullRoot = [IO.Path]::GetFullPath($installRoot).TrimEnd('\') + '\'
    if (-not $fullInstall.StartsWith($fullRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to replace a directory outside $installRoot."
    }
    if (Test-Path -LiteralPath $fullInstall) {
        Remove-Item -LiteralPath $fullInstall -Recurse -Force
    }
    New-Item -ItemType Directory -Path $fullInstall | Out-Null
    Expand-Archive -LiteralPath $zipPath -DestinationPath $fullInstall

    $exe = Join-Path $fullInstall "CodexVoiceInput.exe"
    if (-not (Test-Path -LiteralPath $exe)) {
        throw "Installed executable was not found."
    }
    if (-not $NoShortcut) {
        $desktop = [Environment]::GetFolderPath([Environment+SpecialFolder]::DesktopDirectory)
        $shortcutPath = Join-Path $desktop "Codex Voice Input.lnk"
        $shell = New-Object -ComObject WScript.Shell
        $shortcut = $shell.CreateShortcut($shortcutPath)
        $shortcut.TargetPath = $exe
        $shortcut.WorkingDirectory = $fullInstall
        $shortcut.IconLocation = "$exe,0"
        $shortcut.Description = "Unofficial Codex Desktop voice-to-text input"
        $shortcut.Save()
    }
    Write-Host "Installed Codex Voice Input $($release.tag_name) to $fullInstall"
    if (-not $NoLaunch) {
        Start-Process -FilePath $exe
    }
}
finally {
    if (Test-Path -LiteralPath $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}
