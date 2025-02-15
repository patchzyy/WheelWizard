namespace WheelWizard.Services.Installation;

public class AutoUpdater
{
    public static void CheckForUpdatesAsync()
    {
    #if WINDOWS
            AutoUpdaterWindows.CheckForUpdatesAsync();
    #elif LINUX
            // Linux updater
    #elif MACOS
            // MacOS updater
    #endif
    }
}
