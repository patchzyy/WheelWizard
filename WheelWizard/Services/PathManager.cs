using System;
using System.IO;
using System.Runtime.InteropServices;
using WheelWizard.Helpers;
using WheelWizard.Services.Settings;

namespace WheelWizard.Services;

public static class PathManager
{
    // IMPORTANT: To keep things consistent all paths should be Attrib expressions,
    //            and either end with `FilePath` or `FolderPath`
    
    // pats set by the user
    public static string GameFilePath => (string)SettingsManager.GAME_LOCATION.Get();
    public static string DolphinFilePath => (string)SettingsManager.DOLPHIN_LOCATION.Get();
    public static string UserFolderPath => (string)SettingsManager.USER_FOLDER_PATH.Get();
    
    // Wheel wizard's appdata paths  (dont have to be expressions since they dont depend on user input like the others)
    public static readonly string WheelWizardAppdataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII");
    public static readonly string WheelWizardConfigFilePath = Path.Combine(WheelWizardAppdataPath, "config.json");
    
    // helper paths for folders used across multiple files
    public static string RiivolutionWhWzFolderPath => Path.Combine(LoadFolderPath, "Riivolution", "WheelWizard");
    public static string RetroRewind6FolderPath => Path.Combine(RiivolutionWhWzFolderPath, "RetroRewind6");
    
    //this is not the folder your save file is located in, but its the folder where every Region folder is, so the save file is in SaveFolderPath/Region
    public static string SaveFolderPath => Path.Combine(RiivolutionWhWzFolderPath, "riivolution", "Save" ,"RetroWFC");
    public static string LoadFolderPath => Path.Combine(UserFolderPath, "Load");

    public static string ConfigFolderPath
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(UserFolderPath, "Config");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "/", ".var", "app", "org.DolphinEmu.dolphin-emu", "config", "dolphin-emu");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "/", "Library", "Application Support", "Dolphin", "Config");
            }
            throw new PlatformNotSupportedException("Unsupported operating system");
        }
    }
    public static string WiiFolderPath => Path.Combine(UserFolderPath, "Wii");
    
    
    public static string? TryFindUserFolderPath()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                       "Dolphin Emulator");
        if (FileHelper.DirectoryExists(appDataPath))
            return appDataPath;

        // Macos path
        var libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                       "Library", "Application Support", "Dolphin");
        if (FileHelper.DirectoryExists(libraryPath))
            return libraryPath;

        var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                         "Dolphin Emulator");
        return FileHelper.DirectoryExists(documentsPath) ? documentsPath : null;
    }

    public static string? TryToFindApplicationPath() {

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // Try system wide install on MacOS
            var path = "/Applications/Dolphin.app/Contents/MacOS/Dolphin";
            if (FileHelper.FileExists(path))
                return path;
            // Try user install on MacOS
            path = Path.Combine("~", "Applications", "Dolphin.app", "Contents", "MacOS", "Dolphin");
            if (FileHelper.FileExists(path))
                return path;
        }
        return null;
    }
}
