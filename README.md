<p align="center">
  <img src="./src/VoiceInput.App/Assets/codex-voice-input.png" width="190" alt="Codex Voice Input - Windows Codex voice typing, multilingual speech to text and dictation app">
</p>

<h1 align="center">Codex Voice Input</h1>

<p align="center">
  <strong>把 Codex 变成 Windows 全局多语言语音输入工具</strong>
</p>

<p align="center">
Codex ASR · ChatGPT speech to text · Windows multilingual dictation · Voice typing for almost any text box
</p>

<p align="center">
  <a href="https://github.com/A3Boy/codex-voice-input/releases/latest"><img src="https://img.shields.io/github/v/release/A3Boy/codex-voice-input?logo=github" alt="Latest Codex Voice Input release"></a>
  <a href="https://github.com/A3Boy/codex-voice-input/actions/workflows/build.yml"><img src="https://img.shields.io/github/actions/workflow/status/A3Boy/codex-voice-input/build.yml?branch=main&label=build" alt="Codex Voice Input build status"></a>
  <a href="https://github.com/A3Boy/codex-voice-input/actions/workflows/release.yml"><img src="https://img.shields.io/github/actions/workflow/status/A3Boy/codex-voice-input/release.yml?label=release" alt="Codex Voice Input release status"></a>
  <a href="./LICENSE"><img src="https://img.shields.io/github/license/A3Boy/codex-voice-input" alt="MIT open source license"></a>
  <img src="https://img.shields.io/badge/platform-Windows%2010%2F11%20x64-0078D4?logo=windows" alt="Windows 10 and Windows 11 x64">
</p>

<p align="center">
  <a href="#中文">中文</a> ·
  <a href="#english">English</a> ·
  <a href="https://github.com/A3Boy/codex-voice-input/releases/latest">Download</a>
</p>

> [!WARNING]
> Codex Voice Input 是非官方开源项目，与 OpenAI 无隶属、赞助或背书关系。项目复用逆向研究得到的 Codex Desktop 转写流程，并非公开稳定 API。如果 Codex Desktop 的认证方式、请求头或转写端点发生变化，本项目可能需要跟随更新。

## Screenshots

<p align="center">
  <img src="./docs/screenshots/capsule-idle.png" width="46%" alt="Codex Voice Input floating Windows voice typing capsule">
  <img src="./docs/screenshots/capsule-recording-active.png" width="46%" alt="Codex multilingual speech to text recording waveform">
</p>

<p align="center">
  <img src="./docs/screenshots/capsule-recording-silent.png" width="46%" alt="Windows Codex voice input silent recording state">
  <img src="./docs/screenshots/capsule-result.png" width="46%" alt="Codex speech recognition result preview and copy button">
</p>

# 中文

## 把 Codex 语音转文字带到整个 Windows 桌面

Codex Voice Input 是一款面向 Windows 10 和 Windows 11 的全局语音输入、语音转文字和多语言听写工具。

它复用本机已有的 Codex Desktop 或 Codex CLI 登录状态，将 Codex 使用的 ChatGPT 语音转写能力扩展到浏览器、聊天软件、代码编辑器、IDE、文档、笔记软件和其他常用 Windows 输入框。

不需要先打开网页，不需要手动上传录音，也不需要额外配置 OpenAI API Key。

只需把光标放进输入框，按下全局快捷键，然后直接说话：

1. 按快捷键开始录音。
2. 再按一次停止录音。
3. Codex 自动完成语音识别。
4. 预览转写结果。
5. 一键输入当前窗口或复制到剪贴板。

无论你是在写代码注释、整理需求、回复消息、记录灵感、填写表单，还是输入一段较长的自然语言，Codex Voice Input 都能把语音快速变成可编辑文字。

## 为什么选择 Codex Voice Input

### 多语言自动匹配，无需来回切换识别语言

Codex Voice Input 默认使用自动语言识别。

它不只适用于普通话和英语，还可以处理日语、韩语、法语、德语、西班牙语等多种常见语言。遇到中英混说、英文产品名、技术术语、软件名称和代码相关表达时，也不需要每次录音前手动修改语言设置。

你可以自然地说：

* “帮我把这个 component 改成 responsive layout。”
* “这个 API response 需要增加 error handling。”
* “明天下午 review 一下 Claudio 的 playback queue。”
* “Please summarize this issue，然后翻译成中文。”

语言识别、标点和文本结果由当前 Codex 转写端点自动处理，让多语言语音输入更接近日常说话方式。

### 更适合长句、连续表达和真实工作流

传统语音输入经常更适合短命令，而 Codex Voice Input 更强调完整想法的连续口述。

它适合输入：

* 产品需求和功能说明
* 代码注释和开发文档
* Git 提交说明
* GitHub Issue 和 Bug 报告
* ChatGPT、Claude 和其他 AI 对话内容
* 邮件、聊天回复和工单说明
* 会议记录和个人笔记
* 文章草稿和创作灵感

你不需要刻意把一句话拆成多个短命令，可以直接按照自然语速表达完整想法。

### 在当前应用中直接完成语音输入

Codex Voice Input 不是一个要求你反复切换窗口的音频转写网站。

它以轻量悬浮胶囊的形式常驻桌面，通过全局快捷键随时启动。识别完成后，文字可以直接进入当前获得焦点的输入框。

这意味着你可以在以下应用中继续原来的工作流：

* Chrome、Edge、Firefox 等浏览器
* 微信、QQ、Discord、Slack、Telegram 等聊天软件
* Visual Studio Code、Visual Studio、JetBrains IDE 等开发工具
* Word、WPS、Notion、Obsidian 等文档和笔记工具
* ChatGPT、Claude、Gemini 等 AI 产品
* GitHub、GitLab、Jira 和各种网页表单
* 大多数普通 Windows 文本输入控件

对于不接受模拟输入的特殊窗口，也可以使用复制按钮保留识别结果。

### 复用 Codex 登录，无需额外配置 API Key

Codex Voice Input 直接读取本机：

```text
%USERPROFILE%\.codex\auth.json
```

只要当前电脑已经登录 Codex Desktop 或 Codex CLI，并且账户具备可用的转写能力，就可以开始使用。

你不需要：

* 创建新的 OpenAI API Key
* 在软件中粘贴和管理密钥
* 配置单独的语音识别服务
* 搭建自己的转写服务器
* 在多个软件之间复制音频和识别结果

### 识别完成后先预览，再决定是否输入

普通语音输入经常会把错误结果直接写进当前文档。

Codex Voice Input 会先显示识别结果，让你决定下一步操作：

* 点击结果，将文字输入当前焦点
* 点击复制按钮，把结果放入剪贴板
* 查看最近识别历史
* 取消不满意的识别结果
* 重新录音

这种“先识别、再确认”的流程更适合代码、文档、邮件和其他不希望误输入的场景。

## 核心功能

| 功能             | 说明                                 |
| -------------- | ---------------------------------- |
| Codex 语音转文字    | 复用 Codex Desktop 使用的 ChatGPT 转写端点  |
| Windows 全局语音输入 | 在大多数常用 Windows 输入框中使用              |
| 多语言自动识别        | 自动识别中文、英文及多种常见语言                   |
| 混合语言输入         | 支持包含英文术语、产品名和技术表达的自然口述             |
| 全局快捷键          | 默认 `Ctrl+Alt+Space`，可以在设置中修改       |
| 浮动胶囊 UI        | 置顶显示、自由拖动、贴边隐藏                     |
| 实时音量波形         | 根据真实麦克风音量显示录音状态                    |
| 静音状态反馈         | 没有检测到明显声音时显示静音基线                   |
| 转写结果预览         | 确认结果后再输入当前窗口                       |
| 一键复制           | 将转写结果复制到剪贴板                        |
| 本地识别历史         | 保存最近识别结果，方便再次复制                    |
| 麦克风选择          | 在设置中切换可用录音设备                       |
| 深色模式           | 支持浅色和深色界面                          |
| 系统托盘           | 可以长期驻留后台并快速打开设置                    |
| 单实例运行          | 防止重复托盘图标和快捷键冲突                     |
| 自动清理录音         | 完成、失败或取消后自动删除临时 WAV                |
| 自包含安装          | 安装包已包含所需 .NET 和 Windows App SDK 组件 |

## 适合哪些人

### 开发者和程序员

通过语音快速输入：

* 代码注释
* 功能需求
* README 文档
* Git commit message
* Pull Request 描述
* Bug 复现步骤
* 测试说明
* AI 编程助手提示词

多语言自动匹配特别适合同时包含中文描述、英文变量名、框架名称和技术术语的开发语境。

### ChatGPT、Codex 和 AI 工具重度用户

长时间使用 AI 对话时，键盘输入大量提示词容易疲劳。

Codex Voice Input 可以把完整想法直接口述成文字，再输入 ChatGPT、Claude、Gemini、Codex 或其他 AI 工具，让复杂提示词和长篇上下文输入更快、更自然。

### 内容创作者和写作者

当想法出现得比打字更快时，可以直接说出：

* 文章大纲
* 视频脚本
* 社交媒体文案
* 产品介绍
* 灵感笔记
* 故事情节
* 修改意见

先通过语音完成初稿，再进入编辑器精修。

### 办公和日常沟通用户

适合邮件、即时消息、会议记录、客服工单、网页表单和各种长文本输入场景，减少重复打字。

### 多语言用户

对于需要频繁输入中文、英文和其他语言的用户，不必每次手动选择识别语言，可以直接开始说话。

## 下载与安装

### 方法一：Windows 一键安装包

前往 [Latest Release](https://github.com/A3Boy/codex-voice-input/releases/latest)，下载：

* `CodexVoiceInput-Setup.exe`：推荐版本，双击后按安装向导完成安装。
* `CodexVoiceInput-win-x64.zip`：便携版，解压后直接运行。

一键安装包会创建开始菜单和桌面快捷方式。

安装包和便携版均包含所需的 .NET 8 与 Windows App SDK 运行组件，不需要用户额外安装运行时。

### 方法二：PowerShell 一行安装

```powershell
powershell -ExecutionPolicy Bypass -c "irm https://github.com/A3Boy/codex-voice-input/releases/latest/download/install.ps1 | iex"
```

安装脚本会：

1. 获取最新 GitHub Release。
2. 下载 Windows x64 便携包。
3. 根据 `SHA256SUMS.txt` 校验 ZIP 的 SHA-256。
4. 安装到：

```text
%LOCALAPPDATA%\Programs\CodexVoiceInput
```

5. 创建桌面快捷方式。
6. 启动 Codex Voice Input。

## 快速开始

### 第一步：登录 Codex

安装并登录 Codex Desktop 或 Codex CLI。

确认以下文件存在：

```text
%USERPROFILE%\.codex\auth.json
```

### 第二步：启动 Codex Voice Input

打开应用后，浮动语音胶囊会出现在桌面上，同时应用进入系统托盘。

### 第三步：开始语音输入

1. 点击浏览器、编辑器、聊天软件或文档中的输入框。
2. 按下 `Ctrl+Alt+Space`。
3. 直接说出要输入的内容。
4. 再按一次 `Ctrl+Alt+Space` 停止录音。
5. 等待 Codex 完成语音转文字。
6. 点击识别结果输入当前窗口，或点击复制按钮。

## Codex Voice Input 的工作原理

Codex Desktop 内部会将录音发送到 ChatGPT 后端转写端点并获取文本。

Codex Voice Input 复用了这一流程，并将它包装成 Windows 桌面语音输入工具：

1. 从本机读取 Codex 登录信息。
2. 使用 Windows 麦克风录制 WAV 音频。
3. 请求：

```text
https://chatgpt.com/backend-api/transcribe
```

4. 由转写端点识别语音内容和语言。
5. 在浮动胶囊中显示识别结果。
6. 通过 Win32 `SendInput` 将文本写入当前获得焦点的输入框。

本项目参考并致谢 [Wangnov/codex-asr](https://github.com/Wangnov/codex-asr) 对 Codex 转写流程的研究。

Codex Voice Input 不是 `codex-asr` 的包装器，也不会调用它的可执行文件。桌面应用、录音、浮动 UI、全局快捷键、历史记录、剪贴板和转写请求逻辑均使用 C# 独立实现。

## 隐私与安全

Codex Voice Input 不运营任何中转服务器，也不会把录音、Codex 登录信息或识别历史发送给项目作者。

* Codex 登录信息从本机 `%USERPROFILE%\.codex\auth.json` 读取。
* 应用不会复制或单独保存 Codex 访问令牌。
* 转写时，访问令牌只作为认证信息发送到 ChatGPT 转写端点。
* 麦克风录音直接发送到 ChatGPT 转写端点。
* 录音不会先上传到项目作者控制的服务器。
* 临时 WAV 会在完成、失败或取消后自动删除。
* 应用启动时会清理遗留的旧录音文件。
* 识别历史保存在本机。
* 配置文件和运行日志保存在本机。
* 日志文件设有大小限制，避免无限增长。

请勿在公开 Issue 中上传：

* `.codex/auth.json`
* Codex Access Token
* 私人录音
* 识别历史
* 包含个人路径或私人内容的原始日志

更多安全边界请阅读 [SECURITY.md](SECURITY.md)。

## 系统要求

* Windows 10 2004+ 或 Windows 11
* Windows x64 设备
* 可用的麦克风
* 已安装并登录 Codex Desktop 或 Codex CLI
* 本机存在 `%USERPROFILE%\.codex\auth.json`
* 当前账户可以访问 Codex 使用的转写能力
* 网络可以访问 `chatgpt.com`

## 已知限制

* 本项目是非官方逆向项目，不是 OpenAI 官方产品或公开 API。
* Codex 转写端点、认证格式和请求头可能随 Codex Desktop 更新发生变化。
* 当接口发生变化时，项目可能暂时失效，直到完成适配。
* 当前不是完整的 Windows TSF 输入法。
* 文本输入依赖 Win32 `SendInput`。
* 管理员权限窗口、安全桌面、部分终端和特殊输入控件可能拒绝模拟输入。
* 遇到无法直接输入的窗口时，可以使用复制按钮获取识别结果。
* 多语言混说、专业术语和产品名称的效果可能受到麦克风、环境噪声、网络和转写端点状态影响。
* 当前仅支持 Windows x64。

## 本地数据位置

```text
配置文件：%LOCALAPPDATA%\CodexVoiceInput\config.json
识别历史：%LOCALAPPDATA%\CodexVoiceInput\history.json
日志文件：%LOCALAPPDATA%\CodexVoiceInput\codex-voice-input.log
临时录音：%TEMP%\CodexVoiceInput
安装目录：%LOCALAPPDATA%\Programs\CodexVoiceInput
```

## 从源码构建

需要 Visual Studio 2022 或 Visual Studio Build Tools，并安装：

* .NET desktop development workload
* Windows SDK
* .NET 8 SDK

构建项目：

```powershell
.\build.ps1 -Configuration Debug
```

运行应用：

```powershell
.\run.ps1
```

运行测试：

```powershell
dotnet run --project .\tests\HotkeyContract\HotkeyContract.csproj
dotnet run --project .\tests\TextPostProcessorContract\TextPostProcessorContract.csproj
dotnet run --project .\tests\ClipboardContract\ClipboardContract.csproj
Get-ChildItem .\scripts\test-*-contract.ps1 | ForEach-Object { & $_.FullName }
```

构建发布包：

```powershell
.\package.ps1
```

## 常见问题

### Codex 可以用来进行 Windows 语音输入吗？

Codex Desktop 本身的语音转写能力通常存在于特定应用流程中。Codex Voice Input 将相关转写流程包装成一个 Windows 桌面工具，使其可以通过全局快捷键服务于大多数常用输入框。

### Windows 怎么使用 Codex 语音转文字？

安装并登录 Codex Desktop 或 Codex CLI，然后启动 Codex Voice Input。将光标放入输入框，按 `Ctrl+Alt+Space` 开始录音，再按一次完成 Codex 语音转文字。

### Codex Voice Input 支持哪些语言？

应用默认启用自动语言识别，可以处理中文、英文、日语、韩语、法语、德语、西班牙语等多种常见语言。

中英混说、技术术语、产品名称和代码相关表达也可以直接口述，不需要逐次切换语言。具体结果取决于当前转写端点和录音质量。

### 使用 Codex Voice Input 需要 OpenAI API Key 吗？

不需要额外配置 OpenAI API Key。

项目复用本机现有的 Codex Desktop 或 Codex CLI 登录状态。

### Codex Voice Input 是免费的吗？

Codex Voice Input 本身采用 MIT License 开源发布。

但是使用转写功能需要本机存在可用的 Codex 登录状态，并且当前账户具备访问相关能力的资格。

### 可以在微信、浏览器和 VS Code 中使用吗？

可以在大多数普通 Windows 输入框中使用，包括常见浏览器、聊天软件、文档工具、Visual Studio Code 和多种 IDE。

部分管理员权限窗口、安全桌面、特殊终端或自定义输入控件可能阻止 `SendInput`。遇到这种情况，可以复制识别结果后手动粘贴。

### 它是一个正式的 Windows 输入法吗？

它提供接近系统级输入的使用体验，但目前不是基于 TSF 的完整 Windows 输入法，也不会出现在 Windows 输入法切换列表中。

它通过全局快捷键启动录音，并使用 Win32 输入模拟将识别结果写入当前窗口。

### 录音文件会上传到哪里？

录音会直接发送到 ChatGPT 转写端点，不会经过项目作者控制的中转服务器。

临时 WAV 会在识别完成、失败或取消后删除。

### 这是 OpenAI 官方项目吗？

不是。

Codex Voice Input 是非官方开源项目，与 OpenAI 无隶属、赞助或背书关系。

### 如果 Codex 转写接口失效怎么办？

由于项目使用的是逆向研究得到的非公开流程，接口可能随 Codex Desktop 更新而变化。

项目会根据实际情况尝试跟进新的认证格式、请求头或转写端点，但不承诺长期稳定。

## Roadmap

* 提升 Codex Desktop 更新后的接口适配效率
* 改进第二次启动时的唤醒体验
* 增加更加直观的演示 GIF
* 优化错误提示与登录失效恢复说明
* 继续降低自包含安装包体积
* 改进更多 Windows 应用中的文本输入兼容性

## 致谢

* [Wangnov/codex-asr](https://github.com/Wangnov/codex-asr)：Codex ASR 和转写流程逆向研究参考。
* [NAudio](https://github.com/naudio/NAudio)：Windows 麦克风录音支持。
* Microsoft WinUI 3 / Windows App SDK：Windows 桌面 UI 与系统集成。

## License

[MIT](LICENSE)

Third-party notices are listed in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

---

# English

## Turn Codex into a system-wide multilingual voice typing tool for Windows

Codex Voice Input is an unofficial open-source Windows voice typing, speech-to-text, and multilingual dictation application for Codex users.

It brings the reverse-engineered Codex Desktop transcription flow to browsers, chat applications, code editors, IDEs, documents, note-taking software, AI tools, and most ordinary Windows text fields.

There is no separate OpenAI API key to configure and no transcription server to deploy.

Focus a text box, press the global hotkey, and start speaking:

1. Press the hotkey to record.
2. Press it again to stop.
3. Let the Codex transcription endpoint recognize your speech.
4. Preview the result.
5. Type it into the active application or copy it to the clipboard.

Codex Voice Input is designed for developers, writers, AI power users, multilingual speakers, and anyone who wants faster long-form voice input on Windows.

## Why Codex Voice Input?

### Automatic multilingual voice recognition

Codex Voice Input does not require you to select a language before every recording.

It can automatically recognize Chinese, English, Japanese, Korean, French, German, Spanish, and other commonly used languages.

It is especially useful when natural speech contains:

* Mixed Chinese and English
* Software and product names
* Technical terminology
* API and programming terms
* English identifiers inside another language
* Coding-related phrases

Recognition results depend on microphone quality and the current Codex transcription endpoint, but the automatic workflow makes multilingual dictation significantly more convenient.

### Voice typing for real work

Codex Voice Input is built for more than short voice commands.

Use it to dictate:

* Code comments
* Product requirements
* README documentation
* Git commit messages
* Pull Request descriptions
* Bug reports
* AI prompts
* Emails and chat replies
* Meeting notes
* Articles and content drafts

Speak complete thoughts naturally instead of breaking everything into short commands.

### Use Codex speech to text inside your current application

Codex Voice Input stays available as a lightweight floating capsule.

It can be used with most ordinary input fields in:

* Chrome, Edge, and Firefox
* ChatGPT, Claude, Gemini, and other AI applications
* Visual Studio Code, Visual Studio, and JetBrains IDEs
* Word, WPS, Notion, and Obsidian
* GitHub, GitLab, Jira, and web forms
* Messaging and collaboration applications

For controls that reject simulated typing, the transcription can still be copied to the clipboard.

### No separate OpenAI API key configuration

Codex Voice Input reads the existing local Codex authentication state from:

```text
%USERPROFILE%\.codex\auth.json
```

You do not need to create, paste, or separately manage an OpenAI API key.

## Features

| Feature                      | Description                                                   |
| ---------------------------- | ------------------------------------------------------------- |
| Codex speech to text         | Reuses the transcription flow used by Codex Desktop           |
| Global Windows voice typing  | Works with most standard Windows text fields                  |
| Automatic language detection | Recognizes Chinese, English, and many common languages        |
| Mixed-language dictation     | Useful for technical terms, product names, and coding phrases |
| Global hotkey                | Uses `Ctrl+Alt+Space` by default                              |
| Floating capsule             | Always on top, draggable, and edge-dockable                   |
| Live recording waveform      | Driven by actual microphone volume                            |
| Transcription preview        | Review text before inserting it                               |
| Clipboard support            | Copy transcription results with one click                     |
| Local history                | Reopen and copy recent transcription results                  |
| Microphone selection         | Select an available recording device                          |
| Tray integration             | Keep the application available in the background              |
| Automatic cleanup            | Deletes temporary WAV recordings                              |
| Self-contained packages      | Includes the required .NET and Windows App SDK components     |

## Use Cases

### Developers

Dictate code comments, Git commit messages, requirements, documentation, test instructions, bug reports, and prompts for AI coding assistants.

### AI power users

Speak long prompts and detailed context directly into ChatGPT, Claude, Gemini, Codex, or other AI tools instead of typing everything manually.

### Writers and creators

Capture article drafts, scripts, product descriptions, notes, outlines, and creative ideas while they are still fresh.

### Multilingual users

Move naturally between Chinese, English, and other common languages without manually changing the recognition language for every recording.

## Download and Install

Download the latest Windows release:

https://github.com/A3Boy/codex-voice-input/releases/latest

Available packages:

* `CodexVoiceInput-Setup.exe` — recommended Windows installer.
* `CodexVoiceInput-win-x64.zip` — portable package.

Both packages are self-contained and include the required .NET 8 and Windows App SDK runtime components.

### PowerShell installer

```powershell
powershell -ExecutionPolicy Bypass -c "irm https://github.com/A3Boy/codex-voice-input/releases/latest/download/install.ps1 | iex"
```

The installer script downloads the portable ZIP, verifies its SHA-256 checksum using `SHA256SUMS.txt`, and installs the application to:

```text
%LOCALAPPDATA%\Programs\CodexVoiceInput
```

## Quick Start

1. Install and sign in to Codex Desktop or Codex CLI.
2. Confirm that `%USERPROFILE%\.codex\auth.json` exists.
3. Start Codex Voice Input.
4. Focus the text field where you want to type.
5. Press `Ctrl+Alt+Space` to begin recording.
6. Press the hotkey again to stop and transcribe.
7. Click the result to type it into the focused field, or use the copy button.

## How It Works

1. Reads the local Codex authentication file.
2. Records microphone audio as a WAV file.
3. Sends the authentication token and audio directly to:

```text
https://chatgpt.com/backend-api/transcribe
```

4. Receives the transcription result.
5. Displays a preview in the floating capsule.
6. Inserts the text into the active Windows control using Win32 `SendInput`.

This project was inspired by the Codex ASR research in [Wangnov/codex-asr](https://github.com/Wangnov/codex-asr).

Codex Voice Input is not a wrapper around `codex-asr` and does not call its executable. The Windows desktop application, audio recording, UI, hotkeys, local history, clipboard integration, and transcription request logic are independently implemented in C#.

## Security and Privacy

Codex Voice Input does not operate a relay server and does not send recordings, authentication data, or recognition history to the project author.

* The Codex authentication file remains on your computer.
* The application does not separately store the Codex access token.
* During transcription, the token is sent directly to the ChatGPT transcription endpoint for authentication.
* Recorded audio is sent directly to the same endpoint.
* Temporary WAV files are deleted after completion, failure, or cancellation.
* Old temporary recordings are cleaned when the application starts.
* Recognition history is stored locally.
* Diagnostic logs are stored locally and have a maximum size limit.

Do not upload authentication files, private recordings, recognition history, access tokens, or unsanitized logs to public GitHub issues.

See [SECURITY.md](SECURITY.md) for additional security boundaries.

## Requirements

* Windows 10 2004+ or Windows 11
* Windows x64 device
* Working microphone
* Local Codex Desktop or Codex CLI authentication
* A valid `%USERPROFILE%\.codex\auth.json`
* A Codex account that can access the transcription flow
* Network access to `chatgpt.com`

## Known Limitations

* This is an unofficial reverse-engineered project, not an official OpenAI product or public API.
* The endpoint, authentication format, or request headers may change without notice.
* The application may stop working until it is updated for a new Codex Desktop implementation.
* This is not a full Windows TSF input method.
* Text injection relies on Win32 `SendInput`.
* Elevated windows, secure desktops, terminals, and specialized controls may reject simulated input.
* Transcription quality depends on microphone quality, background noise, network conditions, and the current endpoint.
* Windows x64 is currently the only supported platform.

## FAQ

### Can Codex be used for Windows voice typing?

Codex Voice Input packages the transcription flow used by Codex Desktop into a Windows desktop tool with a global recording hotkey and floating transcription preview.

### Does it support multilingual speech recognition?

Yes. It uses automatic language detection and can recognize Chinese, English, Japanese, Korean, French, German, Spanish, and other common languages.

Mixed-language speech and technical terminology are also supported, although accuracy may vary.

### Does it require an OpenAI API key?

No separate OpenAI API key configuration is required. It reuses the local Codex authentication state.

### Does it work in every Windows text box?

It works with most ordinary Windows input fields. Elevated applications, secure desktops, terminals, and specialized controls may reject simulated input.

### Is it an official OpenAI product?

No. Codex Voice Input is an unofficial open-source project and is not affiliated with, endorsed by, or sponsored by OpenAI.

### Are recordings stored permanently?

No. Temporary recordings are removed after completion, failure, or cancellation, and stale WAV files are cleaned when the application starts.

## Disclaimer

Codex Voice Input is not an official OpenAI product or public API. The reverse-engineered Codex Desktop transcription flow may change or stop working without notice.

## License

[MIT](LICENSE)

See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) for attribution.
