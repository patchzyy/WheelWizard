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
        => AppBuilder.Configure<WheelWizard.Views.App>()
                     .UsePlatformDetect()
                     .WithInterFont()
                     .LogToTrace();

    private static void Setup()
    {
        SettingsManager.Instance.LoadSettings();
        AutoUpdater.CheckForUpdatesAsync();
        UrlProtocolManager.SetWhWzSchemeAsync();
    }

    // TODO: Complete the avalonia transition
    //  - When the whole avalonia views are complete and work, remove the WPFviews folder.
    //  - Remove the 2 threads in the main method, instead it should only have the avalonia app without the thread
    //  - Then go in the csproj, and replace `   <TargetFramework>net7.0-windows</TargetFramework>` with `net7.0` or `net8.0` (just not with the -windows)
    //  - Then remove the `<UseWPF>true</UseWPF>` tag
    //  - Then go int the <ItemGroup> that has the comments about something specific to the OG Wheel wizard wpf application. Filter everything in there what should all be thrown away
    //  - Remove all the page xaml references in this .csproj
    //  - Check what AssemblyInfo.cs is doing there still without WPF
    //  - Move the new App.axaml in the place of App.xaml (in the root directory)
} 
