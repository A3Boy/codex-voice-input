using Microsoft.UI.Xaml;
using VoiceInput.App.Services;

namespace VoiceInput.App;

public partial class App : Application
{
    private MainWindow? window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            window = new MainWindow();
            window.Closed += (_, _) => window.Dispose();
            window.Activate();
            window.HideHostWindow();
            if (Environment.GetCommandLineArgs().Any(argument => argument.Equals("--open-settings", StringComparison.OrdinalIgnoreCase)))
            {
                window.OpenSettingsForDiagnostics();
            }
        }
        catch (Exception error)
        {
            AppDiagnostics.Error("Application startup failed", error);
            throw;
        }
    }
}
