using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace VoiceInput.App.Services;

public sealed class GlobalHotkey : IDisposable
{
    private const int HotkeyId = 0x5649;
    private const int WmHotkey = 0x0312;
    private const int WmQuit = 0x0012;
    private const uint ModNoRepeat = 0x4000;

    private readonly DispatcherQueue dispatcher;
    private readonly HotkeyDefinition hotkey;
    private readonly ManualResetEventSlim ready = new();
    private Thread? thread;
    private uint threadId;
    private Exception? startupError;

    public event EventHandler? Pressed;

    public GlobalHotkey(Window window, HotkeyDefinition hotkey)
    {
        dispatcher = window.DispatcherQueue;
        this.hotkey = hotkey;
    }

    public void Register()
    {
        if (thread is not null) return;

        thread = new Thread(MessageLoop)
        {
            IsBackground = true,
            Name = "VoiceInput.GlobalHotkey",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        ready.Wait(TimeSpan.FromSeconds(3));

        if (startupError is not null)
        {
            throw startupError;
        }

        if (threadId == 0)
        {
            throw new InvalidOperationException("Global hotkey thread did not start.");
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
        _ = PeekMessage(out _, 0, 0, 0, 0);

        if (!RegisterHotKey(0, HotkeyId, hotkey.Modifiers | ModNoRepeat, hotkey.VirtualKey))
        {
            startupError = new InvalidOperationException($"Failed to register {hotkey.Display} hotkey.");
            ready.Set();
            return;
        }

        ready.Set();
        try
        {
            while (GetMessage(out var message, 0, 0, 0) > 0)
            {
                if (message.Message == WmHotkey && message.WParam == (nuint)HotkeyId)
                {
                    dispatcher.TryEnqueue(() => Pressed?.Invoke(this, EventArgs.Empty));
                }
            }
        }
        finally
        {
            UnregisterHotKey(0, HotkeyId);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Msg
    {
        public nint Hwnd;
        public uint Message;
        public nuint WParam;
        public nint LParam;
        public uint Time;
        public int PointX;
        public int PointY;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);

    [DllImport("user32.dll")]
    private static extern int GetMessage(out Msg lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool PeekMessage(out Msg lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostThreadMessage(uint idThread, int msg, nuint wParam, nint lParam);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();
}
