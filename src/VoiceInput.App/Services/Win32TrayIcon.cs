using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using System.Drawing;

namespace VoiceInput.App.Services;

public sealed class Win32TrayIcon : IDisposable
{
    private const int TrayId = 0x5649;
    private const int WmTray = 0x800 + 0x5649;
    private const int WmCommand = 0x0111;
    private const int WmDestroy = 0x0002;
    private const int WmQuit = 0x0012;
    private const int WmLButtonUp = 0x0202;
    private const int WmRButtonUp = 0x0205;
    private const int MenuShow = 1001;
    private const int MenuConfig = 1002;
    private const int MenuLog = 1003;
    private const int MenuExit = 1004;
    private const uint NifMessage = 0x00000001;
    private const uint NifIcon = 0x00000002;
    private const uint NifTip = 0x00000004;
    private const uint NimAdd = 0x00000000;
    private const uint NimModify = 0x00000001;
    private const uint NimDelete = 0x00000002;
    private const uint MfString = 0x00000000;
    private const uint MfSeparator = 0x00000800;
    private const uint TpmRightButton = 0x0002;

    private readonly DispatcherQueue dispatcher;
    private readonly Action show;
    private readonly Action openConfig;
    private readonly Action openLog;
    private readonly Action exit;
    private readonly ManualResetEventSlim ready = new();
    private Thread? thread;
    private uint threadId;
    private nint hwnd;
    private nint icon;
    private Exception? startupError;
    private volatile string tooltip = "Codex Voice Input";

    public Win32TrayIcon(DispatcherQueue dispatcher, Action show, Action openConfig, Action openLog, Action exit)
    {
        this.dispatcher = dispatcher;
        this.show = show;
        this.openConfig = openConfig;
        this.openLog = openLog;
        this.exit = exit;
    }

    public void Start()
    {
        if (thread is not null) return;

        thread = new Thread(MessageLoop)
        {
            IsBackground = true,
            Name = "VoiceInput.Tray",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        ready.Wait(TimeSpan.FromSeconds(3));

        if (startupError is not null)
        {
            throw startupError;
        }
    }

    public void SetTooltip(string text)
    {
        tooltip = text.Length > 120 ? text[..120] : text;
        if (hwnd != 0)
        {
            ShellNotify(NimModify);
        }
    }

    public void Dispose()
    {
        if (threadId != 0)
        {
            PostThreadMessage(threadId, WmQuit, 0, 0);
            threadId = 0;
        }

        if (thread is { IsAlive: true })
        {
            thread.Join(TimeSpan.FromSeconds(1));
        }

        ready.Dispose();
        GC.SuppressFinalize(this);
    }

    private void MessageLoop()
    {
        threadId = GetCurrentThreadId();
        var className = $"VoiceInputTray{Guid.NewGuid():N}";

        var wndProc = new WndProcDelegate(WndProc);
        var wndClass = new WndClass
        {
            ClassName = className,
            WndProc = Marshal.GetFunctionPointerForDelegate(wndProc),
            Instance = GetModuleHandle(null),
        };

        if (RegisterClass(ref wndClass) == 0)
        {
            startupError = new InvalidOperationException("Failed to register tray window class.");
            ready.Set();
            return;
        }

        hwnd = CreateWindowEx(0, className, "Codex Voice Input Tray", 0, 0, 0, 0, 0, 0, 0, wndClass.Instance, 0);
        if (hwnd == 0)
        {
            startupError = new InvalidOperationException("Failed to create tray message window.");
            ready.Set();
            return;
        }

        icon = LoadIcon();
        if (!ShellNotify(NimAdd))
        {
            startupError = new InvalidOperationException("Failed to add tray icon.");
            ready.Set();
            return;
        }

        ready.Set();
        try
        {
            while (GetMessage(out var message, 0, 0, 0) > 0)
            {
                TranslateMessage(ref message);
                DispatchMessage(ref message);
            }
        }
        finally
        {
            ShellNotify(NimDelete);
            if (icon != 0)
            {
                DestroyIcon(icon);
                icon = 0;
            }

            if (hwnd != 0)
            {
                DestroyWindow(hwnd);
                hwnd = 0;
            }
        }

        GC.KeepAlive(wndProc);
    }

    private nint WndProc(nint hWnd, uint message, nuint wParam, nint lParam)
    {
        switch ((int)message)
        {
            case WmTray:
                if (lParam == WmLButtonUp)
                {
                    dispatcher.TryEnqueue(() => show());
                }
                else if (lParam == WmRButtonUp)
                {
                    ShowMenu();
                }
                return 0;
            case WmCommand:
                var command = (int)(wParam & 0xffff);
                if (command == MenuShow)
                {
                    dispatcher.TryEnqueue(() => show());
                }
                else if (command == MenuConfig)
                {
                    dispatcher.TryEnqueue(() => openConfig());
                }
                else if (command == MenuLog)
                {
                    dispatcher.TryEnqueue(() => openLog());
                }
                else if (command == MenuExit)
                {
                    dispatcher.TryEnqueue(() => exit());
                }
                return 0;
            case WmDestroy:
                PostQuitMessage(0);
                return 0;
        }

        return DefWindowProc(hWnd, message, wParam, lParam);
    }

    private void ShowMenu()
    {
        var menu = CreatePopupMenu();
        AppendMenu(menu, MfString, MenuShow, "显示语音输入");
        AppendMenu(menu, MfString, MenuConfig, "打开设置");
        AppendMenu(menu, MfString, MenuLog, "打开日志");
        AppendMenu(menu, MfSeparator, 0, string.Empty);
        AppendMenu(menu, MfString, MenuExit, "退出");
        GetCursorPos(out var point);
        SetForegroundWindow(hwnd);
        TrackPopupMenu(menu, TpmRightButton, point.X, point.Y, 0, hwnd, 0);
        DestroyMenu(menu);
    }

    private bool ShellNotify(uint message)
    {
        var data = new NotifyIconData
        {
            Size = Marshal.SizeOf<NotifyIconData>(),
            Hwnd = hwnd,
            Id = TrayId,
            Flags = NifMessage | NifIcon | NifTip,
            CallbackMessage = WmTray,
            Icon = icon,
            Tip = tooltip,
        };
        return Shell_NotifyIcon(message, ref data);
    }

    private static nint LoadIcon()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "codex-voice-input.png");
        if (File.Exists(path))
        {
            using var bitmap = new Bitmap(path);
            using var resized = new Bitmap(bitmap, new Size(32, 32));
            return resized.GetHicon();
        }

        return LoadIcon(0, new nint(32512));
    }

    private delegate nint WndProcDelegate(nint hWnd, uint message, nuint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WndClass
    {
        public uint Style;
        public nint WndProc;
        public int ClassExtra;
        public int WindowExtra;
        public nint Instance;
        public nint Icon;
        public nint Cursor;
        public nint Background;
        public string? MenuName;
        public string ClassName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NotifyIconData
    {
        public int Size;
        public nint Hwnd;
        public int Id;
        public uint Flags;
        public int CallbackMessage;
        public nint Icon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Tip;
        public uint State;
        public uint StateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Info;
        public uint TimeoutOrVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string InfoTitle;
        public uint InfoFlags;
        public Guid GuidItem;
        public nint BalloonIcon;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Msg
    {
        public nint Hwnd;
        public uint Message;
        public nuint WParam;
        public nint LParam;
        public uint Time;
        public Point Point;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern bool Shell_NotifyIcon(uint dwMessage, ref NotifyIconData lpData);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern ushort RegisterClass(ref WndClass lpWndClass);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint CreateWindowEx(
        uint exStyle,
        string className,
        string windowName,
        uint style,
        int x,
        int y,
        int width,
        int height,
        nint parent,
        nint menu,
        nint instance,
        nint param);

    [DllImport("user32.dll")]
    private static extern nint DefWindowProc(nint hWnd, uint msg, nuint wParam, nint lParam);

    [DllImport("user32.dll")]
    private static extern int GetMessage(out Msg lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage(ref Msg lpMsg);

    [DllImport("user32.dll")]
    private static extern nint DispatchMessage(ref Msg lpMsg);

    [DllImport("user32.dll")]
    private static extern void PostQuitMessage(int exitCode);

    [DllImport("user32.dll")]
    private static extern bool PostThreadMessage(uint idThread, int msg, nuint wParam, nint lParam);

    [DllImport("user32.dll")]
    private static extern bool DestroyWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(nint hIcon);

    [DllImport("user32.dll")]
    private static extern nint CreatePopupMenu();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool AppendMenu(nint hMenu, uint uFlags, int uIdNewItem, string lpNewItem);

    [DllImport("user32.dll")]
    private static extern bool TrackPopupMenu(nint hMenu, uint uFlags, int x, int y, int nReserved, nint hWnd, nint prcRect);

    [DllImport("user32.dll")]
    private static extern bool DestroyMenu(nint hMenu);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point lpPoint);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern nint LoadIcon(nint hInstance, nint lpIconName);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? lpModuleName);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();
}
