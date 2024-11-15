using Avalonia;
using System;

namespace WheelWizard;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine("Application start");
        
        var app = new App();
        app.InitializeComponent();
        app.Run();
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<WheelWizard.Views.App>()
                     .UsePlatformDetect()
                     .WithInterFont()
                     .LogToTrace();
}
