using Microsoft.UI.Xaml;
using VoiceInput.App.Services;

namespace VoiceInput.App;

public partial class App : Application
{
    private const string SingleInstanceMutexName = @"Local\CodexVoiceInput.SingleInstance";

    private MainWindow? window;
    private Mutex? singleInstanceMutex;

    public App()
    {
        InitializeComponent();
        UnhandledException += (_, args) => AppDiagnostics.Error("Unhandled UI exception", args.Exception);
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            singleInstanceMutex = new Mutex(true, SingleInstanceMutexName, out var isFirstInstance);
            if (!isFirstInstance)
            {
                singleInstanceMutex.Dispose();
                singleInstanceMutex = null;
                AppDiagnostics.Info("A second application instance was ignored.");
                Exit();
                return;
            }

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
