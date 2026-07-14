<p align="center">
  <img src="./src/VoiceInput.App/Assets/codex-voice-input.png" width="190" alt="Codex Voice Input app icon">
</p>

<h1 align="center">Codex Voice Input</h1>

<p align="center">
  Windows 全局语音输入工具 · 复用 Codex 登录进行多语种语音转文字
</p>

<p align="center">
  <a href="https://github.com/A3Boy/codex-voice-input/releases/latest"><img src="https://img.shields.io/github/v/release/A3Boy/codex-voice-input?logo=github" alt="Latest Codex Voice Input release"></a>
  <a href="https://github.com/A3Boy/codex-voice-input/actions/workflows/build.yml"><img src="https://img.shields.io/github/actions/workflow/status/A3Boy/codex-voice-input/build.yml?branch=main&label=build" alt="Codex Voice Input build status"></a>
  <a href="https://github.com/A3Boy/codex-voice-input/actions/workflows/release.yml"><img src="https://img.shields.io/github/actions/workflow/status/A3Boy/codex-voice-input/release.yml?label=release" alt="Codex Voice Input release status"></a>
  <a href="./LICENSE"><img src="https://img.shields.io/github/license/A3Boy/codex-voice-input" alt="MIT license"></a>
  <img src="https://img.shields.io/badge/platform-Windows%2010%2F11%20x64-0078D4?logo=windows" alt="Windows 10 and Windows 11 x64">
</p>

<p align="center">
  <a href="#中文">中文</a> ·
  <a href="#english">English</a> ·
  <a href="https://github.com/A3Boy/codex-voice-input/releases/latest">Download</a>
</p>

> [!WARNING]
> Codex Voice Input 是非官方项目，与 OpenAI 无隶属、赞助或背书关系。项目复用逆向研究得到的 Codex Desktop 转写流程，不是公开稳定 API。如果 Codex Desktop 的认证格式、请求头或转写端点发生变化，本项目可能需要跟随更新，也不承诺长期稳定可用。

## Screenshots

<p align="center">
  <img src="./docs/screenshots/capsule-idle.png" width="46%" alt="Codex Voice Input idle floating capsule">
  <img src="./docs/screenshots/capsule-recording-active.png" width="46%" alt="Codex Voice Input microphone-driven recording waveform">
</p>

<p align="center">
  <img src="./docs/screenshots/capsule-recording-silent.png" width="46%" alt="Codex Voice Input silent recording state">
  <img src="./docs/screenshots/capsule-result.png" width="46%" alt="Codex Voice Input transcription preview and copy button">
</p>

# 中文

Codex Voice Input 是一个 Windows 桌面语音输入工具。它读取本机 Codex Desktop 或 Codex CLI 的登录状态，录制麦克风音频，调用 Codex Desktop 使用的 ChatGPT 转写端点，再把识别结果输入到当前获得焦点的文本框。

它适合希望把 Codex 语音转文字能力扩展到日常 Windows 工作流的用户。写文档、口述代码注释、发送聊天消息、记录笔记或填写网页表单时，都可以通过全局快捷键快速完成录音、转写和输入。

## 核心特性

* Windows 全局语音输入：可用于大多数浏览器、聊天软件、编辑器、笔记软件、IDE 和文档输入框。
* 复用 Codex 登录：读取 `%USERPROFILE%\.codex\auth.json`，不需要额外配置 OpenAI API Key。
* 多语种自动识别：默认不需要手动选择语言，可识别中文、英文及多种常见语言。
* 浮动胶囊 UI：轻量置顶、可拖动、可贴边隐藏，适合常驻桌面。
* 实时录音状态：使用麦克风音量驱动动态波形，静音时显示基线状态。
* 识别结果预览：转写完成后可以确认输入，也可以复制识别文本。
* 识别历史：在本机保存最近的识别结果，方便回看和复制。
* 全局快捷键：默认使用 `Ctrl+Alt+Space`，可以在设置中修改。
* 单实例运行：避免出现多个托盘图标、重复快捷键监听和历史文件竞争。
* 自动清理录音：识别完成、失败或取消后删除当前录音，并在启动时清理旧 WAV 文件。
* 自包含安装包：安装包和便携版均包含所需的 .NET 8 与 Windows App SDK 运行组件。

多语言混说、专业术语、产品名称和代码相关表达的实际识别效果，会受到录音质量、网络状态和当前 Codex 转写端点能力的影响。

## 使用场景

* 写作与笔记：把连续口述内容快速转成文字，再进入编辑器整理。
* 编程辅助：口述代码注释、需求说明、测试步骤、提交说明和 Issue 内容。
* 即时聊天：在聊天软件、网页输入框和工单系统中快速输入较长文本。
* 网页表单：减少重复输入地址、说明、反馈和其他自然语言内容。
* 多语言输入：在不手动切换识别语言的情况下输入中文、英文和其他常见语言。

Codex Voice Input 并不是完整的 Windows 输入法，而是通过悬浮胶囊、全局快捷键和 Win32 文本注入，提供接近输入法的桌面语音输入体验。

## 下载与安装

### 方法一：下载安装包

前往 [Latest Release](https://github.com/A3Boy/codex-voice-input/releases/latest)，下载：

* `CodexVoiceInput-Setup.exe`：推荐，按安装向导完成安装，并创建开始菜单和桌面快捷方式。
* `CodexVoiceInput-win-x64.zip`：便携版，解压后直接运行 `CodexVoiceInput.exe`。

安装包和便携版都已经包含 .NET 8 与 Windows App SDK 运行组件，不需要额外安装运行时。

### 方法二：PowerShell 一行安装

```powershell
powershell -ExecutionPolicy Bypass -c "irm https://github.com/A3Boy/codex-voice-input/releases/latest/download/install.ps1 | iex"
```

安装脚本会下载 Windows x64 便携包，根据 `SHA256SUMS.txt` 校验 ZIP 文件的 SHA-256，然后安装到：

```text
%LOCALAPPDATA%\Programs\CodexVoiceInput
```

## 快速开始

1. 安装并登录 Codex Desktop 或 Codex CLI。
2. 确认本机存在 `%USERPROFILE%\.codex\auth.json`。
3. 启动 Codex Voice Input。
4. 将光标放在准备输入文字的位置。
5. 按 `Ctrl+Alt+Space` 开始录音。
6. 再按一次 `Ctrl+Alt+Space` 停止录音并开始转写。
7. 点击识别结果将文字输入当前焦点，或点击复制按钮复制文本。

## 工作原理

Codex Desktop 会把录音发送到 ChatGPT 后端转写端点并返回文本。Codex Voice Input 将这一流程包装成 Windows 桌面语音输入工具：

1. 从本机读取 Codex 登录信息。
2. 使用 Windows 麦克风录制 WAV 音频。
3. 将访问令牌作为认证信息，请求 `https://chatgpt.com/backend-api/transcribe`。
4. 显示转写结果预览。
5. 通过 Win32 `SendInput` 将文本输入当前获得焦点的控件。

本项目参考并致谢 [Wangnov/codex-asr](https://github.com/Wangnov/codex-asr) 对 Codex 转写流程的研究。

Codex Voice Input 不是 `codex-asr` 的包装器，也不会调用其可执行文件。桌面应用、UI、录音、快捷键、历史记录、文本输入和转写请求逻辑均使用 C# 独立实现。

## 隐私与安全

Codex Voice Input 不运营中转服务器，也不会把录音、令牌或识别历史发送给项目作者。

* Codex 登录信息从本机 `%USERPROFILE%\.codex\auth.json` 读取。
* 应用不会复制或单独保存 Codex 访问令牌。
* 发起转写时，访问令牌会作为认证信息直接发送到 ChatGPT 转写端点。
* 录音音频只会直接发送到 ChatGPT 转写端点。
* 临时 WAV 文件会在识别完成、失败或取消后自动删除。
* 应用启动时会清理临时目录中的旧录音文件。
* 识别历史保存在本机 `%LOCALAPPDATA%\CodexVoiceInput\history.json`。
* 日志保存在本机，并限制最大文件大小，避免无限增长。
* 日志可能包含错误信息、文件路径和运行状态，请在公开分享前自行检查并清理。
* 不要在 Issue 中上传 `auth.json`、私人录音、识别历史或未经清理的日志。

更多安全边界见 [SECURITY.md](SECURITY.md)。

## 系统要求

* Windows 10 2004+ 或 Windows 11
* x64 设备
* 可用麦克风
* 本机已登录 Codex Desktop 或 Codex CLI
* 本机存在可用的 Codex 登录状态
* 当前 Codex 订阅可以访问相关转写能力
* 网络可以访问 `chatgpt.com`

## 已知限制

* 这是非官方逆向项目，不是 OpenAI 官方产品或公开 API。
* Codex 转写端点、认证格式或请求头可能在没有通知的情况下发生变化。
* 接口失效时需要根据 Codex Desktop 的最新实现跟随修复。
* 当前不是完整的 TSF 输入法，文本输入依赖 Win32 `SendInput`。
* 某些管理员权限窗口、安全桌面、终端或特殊输入控件可能无法接收模拟输入。
* 如果文本注入失败，仍然可以通过复制按钮获取识别结果。
* 识别质量取决于麦克风、环境噪声、网络状态和当前转写端点。
* 目前只支持 Windows x64。

## 本地文件位置

```text
配置文件：%LOCALAPPDATA%\CodexVoiceInput\config.json
识别历史：%LOCALAPPDATA%\CodexVoiceInput\history.json
日志文件：%LOCALAPPDATA%\CodexVoiceInput\codex-voice-input.log
临时录音：%TEMP%\CodexVoiceInput
安装目录：%LOCALAPPDATA%\Programs\CodexVoiceInput
```

## 从源码构建

需要 Visual Studio 2022 或 Build Tools，并安装：

* .NET desktop development workload
* Windows SDK
* .NET 8 SDK

构建和运行：

```powershell
.\build.ps1 -Configuration Debug
.\run.ps1
```

运行测试：

```powershell
dotnet run --project .\tests\HotkeyContract\HotkeyContract.csproj
dotnet run --project .\tests\TextPostProcessorContract\TextPostProcessorContract.csproj
dotnet run --project .\tests\ClipboardContract\ClipboardContract.csproj
Get-ChildItem .\scripts\test-*-contract.ps1 | ForEach-Object { & $_.FullName }
```

打包：

```powershell
.\package.ps1
```

## FAQ

### 这是 OpenAI 官方项目吗？

不是。Codex Voice Input 是非官方开源项目，与 OpenAI 无隶属、赞助或背书关系。

### 需要 OpenAI API Key 吗？

不需要。项目复用本机 Codex 登录状态，不走公开的 OpenAI API 转写路径。

### 为什么要复用 Codex Desktop 的转写接口？

这个项目的目标就是把 Codex Desktop 使用的转写能力扩展为 Windows 桌面语音输入工具。由于使用的是非公开接口，项目可能需要跟随 Codex Desktop 的更新进行调整。

### 这个项目是正式的 Windows 输入法吗？

不是。它不会出现在 Windows 输入法切换列表中，也没有使用完整的 TSF 输入法架构。它通过全局快捷键录音，再使用 Win32 `SendInput` 将转写结果输入当前控件。

### 可以在所有输入框中使用吗？

不能保证。它可以用于大多数普通 Windows 输入框，但某些管理员权限窗口、安全桌面、终端和特殊控件可能会阻止模拟输入。

### 其他电脑可以使用吗？

可以，但目标电脑需要满足以下条件：

* 使用 Windows x64。
* 已安装 Codex Voice Input。
* 已在该电脑登录 Codex Desktop 或 Codex CLI。
* 本机存在可用的 `%USERPROFILE%\.codex\auth.json`。
* 当前账户和网络可以访问相关转写能力。

Codex 登录状态不会随 Codex Voice Input 安装包迁移。

### 录音文件会保留吗？

正常流程不会长期保留。识别完成、失败或取消后会自动删除当前录音，应用启动时也会清理旧临时 WAV 文件。

### 识别历史保存在哪里？

识别历史只保存在本机：

```text
%LOCALAPPDATA%\CodexVoiceInput\history.json
```

可以在应用设置中查看、复制或清空历史记录。

## Roadmap

* 改进第二次启动时的唤醒体验。
* 提供更清晰的错误提示和恢复建议。
* 增加更完整的演示 GIF。
* 继续优化发布包体积。

## 致谢

* [Wangnov/codex-asr](https://github.com/Wangnov/codex-asr)：Codex ASR 逆向研究参考。
* [NAudio](https://github.com/naudio/NAudio)：Windows 麦克风录音。
* Microsoft WinUI 3 / Windows App SDK：桌面 UI 与 Windows 集成。

## License

[MIT](LICENSE). Third-party notices are listed in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

---

# English

Codex Voice Input is an unofficial Windows voice-input and speech-to-text tool for Codex users.

It turns the reverse-engineered Codex Desktop transcription flow into a lightweight floating dictation capsule. Press a global hotkey to record your microphone, preview the transcription, and type or copy the result without leaving the current Windows application.

## Features

* Global voice input for most standard Windows text fields.
* Reuses the local Codex Desktop or Codex CLI login.
* Does not require separate OpenAI API key configuration.
* Automatic detection for Chinese, English, and various common languages.
* Floating, draggable, always-on-top capsule interface.
* Microphone-level recording waveform and silent-state feedback.
* Transcription preview before typing.
* Copy button and local recognition history.
* Configurable global hotkey, with `Ctrl+Alt+Space` as the default.
* Tray menu, microphone selection, dark mode, and edge docking.
* Automatic cleanup of temporary recordings.
* Self-contained installer and portable ZIP.

Results for mixed-language speech, specialized terminology, product names, and coding-related phrases may vary depending on audio quality and the current Codex transcription endpoint.

## Download

Download the latest Windows release:

https://github.com/A3Boy/codex-voice-input/releases/latest

Available packages:

* `CodexVoiceInput-Setup.exe` for a normal desktop installation.
* `CodexVoiceInput-win-x64.zip` for portable usage.

Both packages include the required .NET 8 and Windows App SDK runtime components.

PowerShell installer:

```powershell
powershell -ExecutionPolicy Bypass -c "irm https://github.com/A3Boy/codex-voice-input/releases/latest/download/install.ps1 | iex"
```

The script downloads the portable ZIP, verifies its SHA-256 checksum using `SHA256SUMS.txt`, and installs the application to:

```text
%LOCALAPPDATA%\Programs\CodexVoiceInput
```

## Quick Start

1. Install and sign in to Codex Desktop or Codex CLI.
2. Confirm that `%USERPROFILE%\.codex\auth.json` exists.
3. Start Codex Voice Input.
4. Focus the text field where you want to enter text.
5. Press `Ctrl+Alt+Space` to start recording.
6. Press the hotkey again to stop and transcribe.
7. Click the transcription to type it into the focused field, or use the copy button.

## How It Works

1. Reads the local Codex authentication file.
2. Records microphone audio as a WAV file.
3. Sends the access token for authentication and the audio directly to `https://chatgpt.com/backend-api/transcribe`.
4. Displays a transcription preview.
5. Types the recognized text into the active Windows input field using Win32 `SendInput`.

This project was inspired by the Codex ASR research in [Wangnov/codex-asr](https://github.com/Wangnov/codex-asr).

Codex Voice Input is not a wrapper around `codex-asr` and does not call its executable. The desktop application, UI, audio recording, hotkeys, local history, text injection, and transcription request logic are independently implemented in C#.

## Security and Privacy

Codex Voice Input does not operate a relay server and does not send recordings, authentication data, or recognition history to the project author.

* The Codex authentication file remains on your machine.
* The application does not copy or separately store the Codex access token.
* During transcription, the access token is sent directly to the ChatGPT transcription endpoint for authentication.
* Recorded audio is sent directly to the same endpoint.
* Temporary WAV files are deleted after completion, failure, or cancellation.
* Old temporary recordings are cleaned when the application starts.
* Recognition history is stored locally at `%LOCALAPPDATA%\CodexVoiceInput\history.json`.
* Diagnostic logs are stored locally and have a maximum size limit.
* Logs may contain error details, local paths, and runtime information. Review and sanitize them before sharing.
* Do not upload Codex authentication files, private audio, recognition history, or unsanitized logs to GitHub issues.

See [SECURITY.md](SECURITY.md) for additional security boundaries.

## Requirements

* Windows 10 2004+ or Windows 11
* x64 device
* Working microphone
* Local Codex Desktop or Codex CLI authentication
* A valid `%USERPROFILE%\.codex\auth.json`
* A Codex subscription that can access the transcription flow
* Network access to `chatgpt.com`

## Known Limitations

* This is an unofficial reverse-engineered project, not an official OpenAI product or public API.
* The endpoint, authentication format, or request headers may change without notice.
* The project may stop working until it is updated to match a new Codex Desktop implementation.
* This is not a full TSF input method.
* Text injection relies on Win32 `SendInput`.
* Elevated windows, secure desktops, terminals, and specialized input controls may reject simulated input.
* Transcription quality depends on microphone quality, background noise, network conditions, and the current endpoint.
* Windows x64 is currently the only supported platform.

## FAQ

### Is this an official OpenAI project?

No. Codex Voice Input is an unofficial open-source project and is not affiliated with, endorsed by, or sponsored by OpenAI.

### Does it require an OpenAI API key?

No. It reuses the local Codex authentication state and does not use the public OpenAI API transcription path.

### Is this a Windows input method?

Not in the TSF sense. It does not appear in the Windows input-language switcher. It records through a global hotkey and inserts recognized text using Win32 input simulation.

### Does it work in every text box?

No guarantee is made. It works with most ordinary Windows text fields, but elevated applications, secure desktops, terminals, and specialized controls may block simulated input.

### Are recordings stored permanently?

No. Temporary recordings are deleted after transcription, failure, or cancellation, and stale WAV files are cleaned when the application starts.

## Disclaimer

This is not an official OpenAI product or API. The reverse-engineered Codex Desktop transcription flow can change or stop working without notice.

## License

[MIT](LICENSE). See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) for attribution.
