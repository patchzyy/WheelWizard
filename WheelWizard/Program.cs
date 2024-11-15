using Avalonia;
using System;

namespace WheelWizard;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine("Application start");
        
        var app = new WheelWizard.App();
        app.InitializeComponent();
        app.Run();
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<WheelWizard.Views.App>()
                     .UsePlatformDetect()
                     .WithInterFont()
                     .LogToTrace();
    
    // TODO: Complete the avalonia transition
    //  - When the whole avalonia views are complete and work, remove the WPFviews folder.
    //  - Then go in the csproj, and replace `   <TargetFramework>net7.0-windows</TargetFramework>` with `net7.0` or `net8.0` (just not with the -windows)
    //  - Then remove the `<UseWPF>true</UseWPF>` tag
    //  - Then go int the <ItemGroup> that has the comments about something specific to the OG Wheel wizard wpf application. Filter everything in there what should all be thrown away
    //  - Remove all the page xaml references in this .csproj
} 
