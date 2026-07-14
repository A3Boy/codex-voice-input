# Codex Voice Input

Windows 原生浮动语音输入工具。项目通过逆向研究 Codex Desktop 的转写请求流程，读取本机 Codex 登录状态，把麦克风语音转换成文字并输入到当前光标位置。

> 非官方项目，与 OpenAI 无隶属、赞助或背书关系。转写接口未公开，可能随 Codex 更新而失效。

## 功能

- WinUI 3 + Win32 透明分层胶囊窗口
- 全局快捷键，一键开始与结束录音
- 实时麦克风音量驱动的多层动态波形
- Codex 自动语言识别，无需 OpenAI API Key
- 识别结果预览、复制、历史记录与当前焦点输入
- 左右屏幕边缘吸附和竖向折叠
- 麦克风与快捷键即时切换
- 完成、失败或取消后自动删除临时录音
- 深色模式和托盘菜单

## 工作原理

应用读取 `%USERPROFILE%\.codex\auth.json` 中当前用户的 Codex 登录令牌，向 `https://chatgpt.com/backend-api/transcribe` 发送录音。令牌不会写入应用配置、识别历史、日志或发布包。

技术路线参考并致谢 [Wangnov/codex-asr](https://github.com/Wangnov/codex-asr)。本项目不是 Rust 包装器，也不依赖 `codex-asr` 可执行文件；桌面应用和请求逻辑使用 C# 独立实现。许可证信息见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)。

## 系统要求

- Windows 10 2004+ 或 Windows 11，x64
- .NET 8 Desktop Runtime
- 已安装并登录 Codex Desktop/CLI，且存在 `%USERPROFILE%\.codex\auth.json`
- 可用的 Codex 订阅与网络连接

## 安装

从 GitHub Releases 下载 `CodexVoiceInput-win-x64.zip`，解压后运行 `CodexVoiceInput.exe`。

从源码安装并创建桌面快捷方式：

```powershell
.\package.ps1
```

开机启动：

```powershell
.\install-startup.ps1
```

## 构建

需要 Visual Studio 2022 或 Build Tools，包含：

- .NET desktop development
- Desktop development with C++
- Windows 10/11 SDK
- .NET 8 SDK

```powershell
.\build.ps1 -Configuration Debug
.\run.ps1
```

运行测试：

```powershell
dotnet run --project .\tests\HotkeyContract\HotkeyContract.csproj
Get-ChildItem .\scripts\test-*-contract.ps1 | ForEach-Object { & $_.FullName }
```

## 本地数据

- 配置：`%LOCALAPPDATA%\CodexVoiceInput\config.json`
- 识别历史：`%LOCALAPPDATA%\CodexVoiceInput\history.json`
- 日志：`%LOCALAPPDATA%\CodexVoiceInput\codex-voice-input.log`
- 临时录音：`%TEMP%\CodexVoiceInput`，转写结束后自动删除

提交问题前请阅读 [SECURITY.md](SECURITY.md)，不要上传认证文件、个人录音、识别历史或未经清理的日志。

## License

[MIT](LICENSE)
