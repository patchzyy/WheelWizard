namespace WheelWizard.Services.Installation;

public class AutoUpdater
{
    
    public const string CurrentVersion = "1.0.0";
    public static void CheckForUpdatesAsync()
    {
    #if WINDOWS
            AutoUpdaterWindows.CheckForUpdatesAsync();
    #elif LINUX
            AutoUpdaterLinux.CheckForUpdatesAsync();
    #elif MACOS
            // MacOS updater
    #endif
    }
}
