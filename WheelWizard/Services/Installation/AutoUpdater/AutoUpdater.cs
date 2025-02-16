using System;

namespace WheelWizard.Services.Installation.AutoUpdater;

public class AutoUpdater
{
    public const string CurrentVersion = "2.0.0";
    public static async void CheckForUpdatesAsync()
    {
        IUpdaterPlatform? autoUpdaterPlatform = null;
#if WINDOWS
            autoUpdaterPlatform = new AutoUpdaterWindows();
#elif LINUX
            autoUpdaterPlatform = new AutoUpdaterLinux();
#elif MACOS
            // MacOS updater
#elif DEBUG
        autoUpdaterPlatform = new AutoUpdaterWindows();
#endif
        if (autoUpdaterPlatform == null)
            throw new PlatformNotSupportedException("The current platform is not supported by the auto updater.");
        IAutoUpdateHandler updateHandler = new AutoUpdaterHandler(autoUpdaterPlatform);
        
        //todo: how to run this in a background thread?
        await updateHandler.CheckForUpdatesAsync();
    }
}
