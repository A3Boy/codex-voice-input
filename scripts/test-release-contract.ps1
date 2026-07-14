$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$installer = Get-Content -Raw (Join-Path $root "install.ps1")
$releaseWorkflow = Get-Content -Raw (Join-Path $root ".github\workflows\release.yml")

if ($installer -notmatch 'Where-Object\s*\{\s*\$_\s+-match\s+''\\s\+CodexVoiceInput-win-x64\\\.zip\$''\s*\}') {
    throw "install.ps1 must select the ZIP checksum by filename instead of assuming SHA256SUMS order."
}

if ($installer -match 'Select-Object\s+-First\s+1\)\s+-split\s+''\\s\+''') {
    throw "install.ps1 still appears to read the first SHA256SUMS line directly."
}

if ($releaseWorkflow -match '(?m)^\s*workflow_dispatch\s*:') {
    throw "release.yml must not support manual workflow_dispatch releases."
}

Write-Host "Release contract passed."
