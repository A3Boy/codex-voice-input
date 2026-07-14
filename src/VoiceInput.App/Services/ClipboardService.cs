using System.Runtime.InteropServices;
using System.Text;

namespace VoiceInput.App.Services;

public static class ClipboardService
{
    private const uint CfUnicodeText = 13;
    private const uint GmemMoveable = 0x0002;
    private const uint GmemZeroInit = 0x0040;

    public static bool TrySetText(string text, out string? error)
    {
        error = null;
        if (string.IsNullOrEmpty(text))
        {
            error = "没有可复制的文本。";
            return false;
        }

        for (var attempt = 0; attempt < 8; attempt++)
        {
            if (TrySetTextOnce(text, out error))
            {
                return true;
            }
            Thread.Sleep(25);
        }

        return false;
    }

    private static bool TrySetTextOnce(string text, out string? error)
    {
        error = null;
        if (!OpenClipboard(0))
        {
            error = $"无法打开剪贴板（Win32 {Marshal.GetLastWin32Error()}）。";
            return false;
        }

        nint memory = 0;
        var clipboardOwnsMemory = false;
        try
        {
            var bytes = Encoding.Unicode.GetBytes(text + '\0');
            memory = GlobalAlloc(GmemMoveable | GmemZeroInit, (nuint)bytes.Length);
            if (memory == 0)
            {
                error = "无法分配剪贴板内存。";
                return false;
            }

            var pointer = GlobalLock(memory);
            if (pointer == 0)
            {
                error = "无法写入剪贴板内存。";
                return false;
            }
            try
            {
                Marshal.Copy(bytes, 0, pointer, bytes.Length);
            }
            finally
            {
                GlobalUnlock(memory);
            }

            if (!EmptyClipboard())
            {
                error = $"无法清空剪贴板（Win32 {Marshal.GetLastWin32Error()}）。";
                return false;
            }
            if (SetClipboardData(CfUnicodeText, memory) == 0)
            {
                error = $"无法设置剪贴板文本（Win32 {Marshal.GetLastWin32Error()}）。";
                return false;
            }

            clipboardOwnsMemory = true;
            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }
        finally
        {
            CloseClipboard();
            if (memory != 0 && !clipboardOwnsMemory)
            {
                GlobalFree(memory);
            }
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool OpenClipboard(nint owner);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EmptyClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetClipboardData(uint format, nint memory);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalAlloc(uint flags, nuint bytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalLock(nint memory);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GlobalUnlock(nint memory);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalFree(nint memory);
}
