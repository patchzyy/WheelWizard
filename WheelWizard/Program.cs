using Avalonia;
using System;
using WheelWizard.Services.Installation;
using WheelWizard.Services.Settings;
using WheelWizard.Services.UrlProtocol;

namespace WheelWizard;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine("Application start");
        Setup();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<Views.App>()
                     .UsePlatformDetect()
                     .WithInterFont()
                     .LogToTrace();

    private static void Setup()
    {
        SettingsManager.Instance.LoadSettings();
        AutoUpdater.CheckForUpdatesAsync();
        UrlProtocolManager.SetWhWzSchemeAsync();
    }
} 
