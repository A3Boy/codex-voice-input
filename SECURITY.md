# Security

## Authentication boundary

Codex Voice Input reads the current user's Codex authentication file from
`%USERPROFILE%\.codex\auth.json`. The token is used only to call the ChatGPT
transcription endpoint and is never copied into this repository, application
configuration, history, diagnostics, or release archives.

Never publish or attach `auth.json`, `%LOCALAPPDATA%\CodexVoiceInput`, voice
recordings, or diagnostic logs to an issue.

## Reporting a vulnerability

Please report vulnerabilities through GitHub private vulnerability reporting
instead of a public issue. Include reproduction steps without authentication
tokens or personal audio.

## Unofficial API warning

This project relies on an undocumented Codex Desktop request flow. Endpoint,
authentication, and request-header behavior can change without notice.
