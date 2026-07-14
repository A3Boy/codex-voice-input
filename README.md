<p align="center">
  <img src="./src/VoiceInput.App/Assets/codex-voice-input.png" width="220" alt="Codex Voice Input icon">
</p>

<h1 align="center">Codex Voice Input</h1>

<p align="center">
  Windows 原生浮动语音输入工具，复用本机 Codex 登录进行语音转文字。
</p>

<p align="center">
  <a href="https://github.com/A3Boy/codex-voice-input/releases/latest"><img src="https://img.shields.io/github/v/release/A3Boy/codex-voice-input?logo=github" alt="Latest release"></a>
  <a href="https://github.com/A3Boy/codex-voice-input/actions/workflows/build.yml"><img src="https://img.shields.io/github/actions/workflow/status/A3Boy/codex-voice-input/build.yml?branch=main&label=build" alt="Build"></a>
  <a href="https://github.com/A3Boy/codex-voice-input/actions/workflows/release.yml"><img src="https://img.shields.io/github/actions/workflow/status/A3Boy/codex-voice-input/release.yml?label=release" alt="Release"></a>
  <a href="./LICENSE"><img src="https://img.shields.io/github/license/A3Boy/codex-voice-input" alt="MIT license"></a>
  <img src="https://img.shields.io/badge/platform-Windows%20x64-0078D4?logo=windows" alt="Windows x64">
</p>

<p align="center">
  <a href="#中文">中文</a> · <a href="#english">English</a>
</p>

> [!WARNING]
> 非官方项目，与 OpenAI 无隶属、赞助或背书关系。项目使用逆向研究得到的 Codex Desktop 转写流程，不是公开稳定 API，可能随 Codex 更新而失效。

# 中文

Codex Desktop 会把语音录音发送到 `https://chatgpt.com/backend-api/transcribe` 并返回文本。Codex Voice Input 将这个流程实现为一个 Windows 浮动输入胶囊：按下全局快捷键开始录音，再次按下完成识别，然后把结果写入当前光标位置。

## 适合谁用

- 已在 Codex Desktop 或 Codex CLI 登录 ChatGPT
- 希望在任意 Windows 输入框中使用 Codex 语音转文字
- 不想配置 OpenAI API Key 或单独购买转写 API
- 接受逆向接口可能变化，需要跟随 Codex 更新

## 安装

### PowerShell 一行安装

```powershell
powershell -ExecutionPolicy Bypass -c "irm https://github.com/A3Boy/codex-voice-input/releases/latest/download/install.ps1 | iex"
```

安装脚本会下载 Windows x64 ZIP、校验 `SHA256SUMS.txt`，安装到 `%LOCALAPPDATA%\Programs\CodexVoiceInput`，创建桌面快捷方式并启动应用。

### 直接下载

从 [最新 GitHub Release](https://github.com/A3Boy/codex-voice-input/releases/latest) 下载 `CodexVoiceInput-win-x64.zip`，解压后运行 `CodexVoiceInput.exe`。

## 快速上手

1. 安装并登录 Codex Desktop/CLI，确认 `%USERPROFILE%\.codex\auth.json` 存在。
2. 启动 Codex Voice Input。
3. 按 `Ctrl+Alt+Space` 开始录音，再按一次停止并转写。
4. 预览识别结果后点击输入，或点击复制按钮。

设置页可以即时切换麦克风、全局快捷键和深色模式。快捷键必须包含至少一个修饰键和一个普通按键，例如 `Ctrl+Alt+K`。

## 功能

- WinUI 3 + Win32 透明分层胶囊窗口
- 实时麦克风音量驱动的多层动态波形
- Codex 自动语言识别，无需 OpenAI API Key
- 识别结果预览、复制、历史记录与当前焦点输入
- 左右屏幕边缘吸附和竖向折叠
- 麦克风与全局快捷键即时切换
- 完成、失败或取消后自动删除临时录音
- 深色模式、托盘菜单和开机启动脚本

## 认证模型与隐私

应用默认读取 `%USERPROFILE%\.codex\auth.json` 中当前用户的 Codex 登录令牌，并直接向 ChatGPT 转写端点发送录音。

- 不会把令牌复制到应用配置、日志、历史或 Release 包
- 不会把录音发送到项目作者控制的服务器
- 临时 WAV 在完成、失败或取消后自动删除
- 不要在 Issue 中上传 `auth.json`、个人录音或未经清理的日志

完整说明见 [SECURITY.md](SECURITY.md)。

## 已知边界

- 仅支持 Windows 10 2004+ / Windows 11 x64
- 需要 .NET 8 Desktop Runtime
- 需要本机有效的 Codex 登录与可用订阅
- 转写端点、认证格式或请求头可能无预告变化
- 当前不是完整 TSF 输入法，文本通过 Win32 `SendInput` 写入当前焦点

## 从源码构建

需要 Visual Studio 2022 或 Build Tools，安装 `.NET desktop development`、`Desktop development with C++`、Windows SDK 和 .NET 8 SDK。

```powershell
.\build.ps1 -Configuration Debug
.\run.ps1
```

运行测试与打包：

```powershell
dotnet run --project .\tests\HotkeyContract\HotkeyContract.csproj
Get-ChildItem .\scripts\test-*-contract.ps1 | ForEach-Object { & $_.FullName }
.\package.ps1
```

本地数据：

- 配置：`%LOCALAPPDATA%\CodexVoiceInput\config.json`
- 识别历史：`%LOCALAPPDATA%\CodexVoiceInput\history.json`
- 日志：`%LOCALAPPDATA%\CodexVoiceInput\codex-voice-input.log`
- 临时录音：`%TEMP%\CodexVoiceInput`

## 技术来源

认证与转写技术路线参考并致谢 [Wangnov/codex-asr](https://github.com/Wangnov/codex-asr)。本项目不是 Rust 包装器，也不依赖 `codex-asr` 可执行文件；桌面应用和请求逻辑使用 C# 独立实现。许可证信息见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)。

---

# English

Codex Voice Input is an unofficial Windows floating voice-input capsule. It reuses the current user's local Codex authentication, records microphone audio, calls the reverse-engineered Codex Desktop transcription endpoint, previews the result, and types it into the currently focused control.

## Requirements

- Windows 10 2004+ or Windows 11 x64
- .NET 8 Desktop Runtime
- A local Codex Desktop/CLI login at `%USERPROFILE%\.codex\auth.json`
- An eligible Codex subscription and network access to `chatgpt.com`

## Install

```powershell
powershell -ExecutionPolicy Bypass -c "irm https://github.com/A3Boy/codex-voice-input/releases/latest/download/install.ps1 | iex"
```

The installer verifies the release SHA-256 checksum before replacing `%LOCALAPPDATA%\Programs\CodexVoiceInput`.

## Security boundary

Authentication stays on the local machine and is sent only to the ChatGPT transcription endpoint. The project does not operate a relay server. Never attach Codex auth files, private audio, recognition history, or raw logs to GitHub issues.

## Disclaimer

This is not an official OpenAI API or product. The endpoint and authentication behavior can change without notice. Codex Voice Input is not affiliated with, endorsed by, or sponsored by OpenAI.

## License

[MIT](LICENSE). See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) for upstream attribution.
