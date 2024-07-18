using System;
using System.IO;
using System.Windows;

namespace CT_MKWII_WPF.Utilities.Configuration;

public class PathManager
{
    public static string GetGameLocation() => ConfigManager.GetConfig().GameLocation;
    public static string GetDolphinLocation() => ConfigManager.GetConfig().DolphinLocation;
    public static string GetUserPathLocation() => ConfigManager.GetConfig().UserFolderPath;
    public static bool GetForceWiimote() => ConfigManager.GetConfig().ForceWiimote;
    
    public static string GetLoadPathLocation() => Path.Combine(ConfigManager.GetConfig().UserFolderPath, "Load");

    public static string FindWiiMoteNew()
    {
        var folderPath = GetUserPathLocation();
        var configFolder = Path.Combine(folderPath, "Config");
        var wiimoteFile = Path.Combine(configFolder, "WiimoteNew.ini");

        if (File.Exists(wiimoteFile))
            return wiimoteFile;
        MessageBox.Show($"Could not find WiimoteNew file, tried looking in {wiimoteFile}", "Error", MessageBoxButton.OK,
            MessageBoxImage.Error);
        return string.Empty;
    }

    public static string FindGfxFile()
    {
        var folderPath = GetUserPathLocation();
        var configFolder = Path.Combine(folderPath, "Config");
        var gfxFile = Path.Combine(configFolder, "GFX.ini");
        if (File.Exists(gfxFile))
            return gfxFile;
        MessageBox.Show($"Could not find GFX file, tried looking in {gfxFile}", "Error", MessageBoxButton.OK,
            MessageBoxImage.Error);
        return string.Empty;
    }
}