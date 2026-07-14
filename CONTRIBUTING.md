# Contributing

Contributions are welcome through issues and pull requests.

1. Do not include Codex tokens, `auth.json`, audio recordings, histories, or logs.
2. Keep the project Windows x64 focused unless a platform proposal includes a tested implementation.
3. Run the contract tests and a Release build before opening a pull request.

```powershell
dotnet run --project .\tests\HotkeyContract\HotkeyContract.csproj
Get-ChildItem .\scripts\test-*-contract.ps1 | ForEach-Object { & $_.FullName }
.\build.ps1 -Configuration Release
```
