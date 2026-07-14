using VoiceInput.App.Services;

if (HotkeyDefinition.TryParse("Ctrl+Alt", out _, out var modifierOnlyError))
{
    throw new InvalidOperationException("Modifier-only shortcuts must be rejected.");
}

if (HotkeyDefinition.TryParse("Ctrl+A+B", out _, out var multipleKeyError))
{
    throw new InvalidOperationException("Multiple ordinary keys must be rejected.");
}
if (!multipleKeyError.Contains("一个普通按键", StringComparison.Ordinal))
{
    throw new InvalidOperationException($"Unexpected multiple-key error: {multipleKeyError}");
}
if (!modifierOnlyError.Contains("普通按键", StringComparison.Ordinal))
{
    throw new InvalidOperationException("Modifier-only validation must explain that a trigger key is required.");
}
if (!HotkeyDefinition.TryParse("ctl + alt + k", out var letterShortcut, out var letterError))
{
    throw new InvalidOperationException($"Valid letter shortcut was rejected: {letterError}");
}
if (letterShortcut.Display != "Ctrl+Alt+K" || letterShortcut.VirtualKey != 'K')
{
    throw new InvalidOperationException("Letter shortcut was not normalized correctly.");
}
if (!HotkeyDefinition.TryParse("shift+ctrl+f12", out var functionShortcut, out var functionError))
{
    throw new InvalidOperationException($"Valid function shortcut was rejected: {functionError}");
}
if (functionShortcut.Display != "Ctrl+Shift+F12")
{
    throw new InvalidOperationException("Function shortcut was not normalized correctly.");
}

Console.WriteLine("hotkey-contract=pass");
