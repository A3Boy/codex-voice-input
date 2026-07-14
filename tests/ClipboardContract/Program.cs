using VoiceInput.App.Services;

var expected = $"clipboard-contract-{Guid.NewGuid():N}";
if (!ClipboardService.TrySetText(expected, out var error))
{
    throw new InvalidOperationException($"Clipboard write failed: {error}");
}

var process = new System.Diagnostics.Process
{
    StartInfo = new System.Diagnostics.ProcessStartInfo
    {
        FileName = "powershell.exe",
        Arguments = "-NoProfile -Command Get-Clipboard",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    },
};
process.Start();
var actual = process.StandardOutput.ReadToEnd().Trim();
process.WaitForExit();
if (process.ExitCode != 0 || actual != expected)
{
    throw new InvalidOperationException($"Clipboard round trip failed. Expected '{expected}', received '{actual}'.");
}

Console.WriteLine("clipboard-contract=pass");
