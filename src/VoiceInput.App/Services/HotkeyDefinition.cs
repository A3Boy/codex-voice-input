namespace VoiceInput.App.Services;

public sealed record HotkeyDefinition(uint Modifiers, uint VirtualKey, string Display)
{
    public static HotkeyDefinition Default { get; } = new(0x0001 | 0x0002, 0x20, "Ctrl+Alt+Space");

    public static HotkeyDefinition Parse(string value)
    {
        return TryParse(value, out var definition, out _) ? definition : Default;
    }

    public static bool TryParse(string value, out HotkeyDefinition definition, out string error)
    {
        definition = Default;
        error = string.Empty;
        var modifiers = 0u;
        uint? key = null;
        string? keyDisplay = null;
        var parts = value
            .Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.ToLowerInvariant())
            .ToArray();

        if (parts.Length == 0)
        {
            error = "请输入快捷键，例如 Ctrl+Alt+Space。";
            return false;
        }

        foreach (var part in parts)
        {
            switch (part)
            {
                case "ctrl":
                case "control":
                case "ctl":
                    modifiers |= 0x0002;
                    break;
                case "alt":
                    modifiers |= 0x0001;
                    break;
                case "shift":
                    modifiers |= 0x0004;
                    break;
                case "win":
                case "windows":
                    modifiers |= 0x0008;
                    break;
                case "space":
                    if (key is not null)
                    {
                        error = "快捷键只能包含一个普通按键。";
                        return false;
                    }
                    key = 0x20;
                    keyDisplay = "Space";
                    break;
                default:
                    if (part.Length == 1 && char.IsLetterOrDigit(part[0]))
                    {
                        if (key is not null)
                        {
                            error = "快捷键只能包含一个普通按键。";
                            return false;
                        }
                        key = char.ToUpperInvariant(part[0]);
                        keyDisplay = part.ToUpperInvariant();
                    }
                    else if (part.StartsWith('f') && int.TryParse(part[1..], out var fKey) && fKey is >= 1 and <= 24)
                    {
                        if (key is not null)
                        {
                            error = "快捷键只能包含一个普通按键。";
                            return false;
                        }
                        key = (uint)(0x70 + fKey - 1);
                        keyDisplay = $"F{fKey}";
                    }
                    else
                    {
                        error = $"无法识别按键“{part}”。请使用字母、数字、F1-F24 或 Space。";
                        return false;
                    }
                    break;
            }
        }

        if (modifiers == 0)
        {
            error = "快捷键至少需要 Ctrl、Alt、Shift 或 Win 中的一个修饰键。";
            return false;
        }
        if (key is null)
        {
            error = "快捷键还需要一个普通按键，例如 Ctrl+Alt+Space。";
            return false;
        }

        var displayParts = new List<string>();
        if ((modifiers & 0x0002) != 0) displayParts.Add("Ctrl");
        if ((modifiers & 0x0001) != 0) displayParts.Add("Alt");
        if ((modifiers & 0x0004) != 0) displayParts.Add("Shift");
        if ((modifiers & 0x0008) != 0) displayParts.Add("Win");
        displayParts.Add(keyDisplay!);

        definition = new HotkeyDefinition(modifiers, key.Value, string.Join('+', displayParts));
        return true;
    }
}
