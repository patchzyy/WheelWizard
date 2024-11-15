using Avalonia;
using System;

namespace CT_MKWII_WPF;

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
        => AppBuilder.Configure<CT_MKWII_WPF.AvaloniaViews.App>()
                     .UsePlatformDetect()
                     .WithInterFont()
                     .LogToTrace();
}
