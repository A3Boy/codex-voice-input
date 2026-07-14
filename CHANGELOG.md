# Changelog

## 0.1.2 - 2026-07-14

### Fixed

- Prevent clipboard contention or WinRT clipboard failures from terminating the application.
- Replace direct WinRT clipboard writes with a retried Win32 Unicode clipboard service.
- Preserve literal words such as `空格`, `逗号`, and `句号` instead of replacing them globally.
- Reject shortcuts containing multiple ordinary keys and suppress key-repeat hotkey events.
- Map Codex authentication, throttling, and other HTTP failures from status codes.
- Delete stale temporary WAV files at startup and cap the diagnostic log at 5 MB.

### Added

- Add a self-contained one-click Windows installer with desktop and Start menu shortcuts.
- Bundle the .NET 8 runtime in both installer and portable distributions.
- Enforce a lightweight named-mutex single instance.

### Removed

- Remove the unused native and TSF placeholder projects.

## 0.1.1 - 2026-07-14

### Added

- Original Codex Voice Input application icon and consistent product naming.
- Public Windows CI and tag-driven GitHub Release automation.
- SHA-256 checksums and a PowerShell installer for release assets.
- Chinese and English project documentation, security policy, contribution guide, and issue templates.

### Changed

- Extracted the application from the Claudio monorepo into a standalone repository.
- Made build scripts independent from private workspace paths and local NuGet configuration.

## 0.1.0 - 2026-07-14

Initial public Windows release.

### Added

- Floating WinUI 3 voice-input capsule with edge docking.
- Live microphone waveform, Codex transcription, preview, copy, history, and text injection.
- Runtime hotkey and microphone switching.
- Automatic temporary recording cleanup.
