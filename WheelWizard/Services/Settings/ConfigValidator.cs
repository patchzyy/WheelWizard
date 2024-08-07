﻿using CT_MKWII_WPF.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace CT_MKWII_WPF.Services.Settings;

public static class ConfigValidator
{
    private static string modConfigFilePath => Path.Combine(PathManager.WheelWizardAppdataPath, "Mods", "modconfig.json");
    
    private static bool DoesConfigExist() => File.Exists(PathManager.WheelWizardConfigFilePath);
    public static bool ConfigCorrectAndExists() => DoesConfigExist() && SetupCorrectly();
    
    public static bool SetupCorrectly()
    {
        var config = ConfigManager.GetConfig();
        if (config.UserFolderPath == null) return false;
        var gfxFile = Path.Combine(config.UserFolderPath, "Config", "GFX.ini");
        return File.Exists(config.DolphinLocation) &&
               File.Exists(config.GameLocation) &&
               File.Exists(gfxFile);
    }
    
    public static bool IsConfigFileFinishedSettingUp()
    {
        var config = ConfigManager.GetConfig();
        return Directory.Exists(config.UserFolderPath) && File.Exists(config.DolphinLocation) &&
               File.Exists(config.GameLocation);
    }
    
    public static bool isValidFavoriteUser(int favoriteUser)
    {
        var config = ConfigManager.GetConfig();
        return favoriteUser is >= 0 and <= 3;
    }

    public static ModData[] GetMods()
    {
        if (!File.Exists(modConfigFilePath))
            return Array.Empty<ModData>();

        var json = File.ReadAllText(modConfigFilePath);
        var mods = JsonConvert.DeserializeObject<ModData[]>(json);
        return mods ?? Array.Empty<ModData>();
    }

    public static void SaveWiimoteSettings(bool value)
    {
        var config = ConfigManager.GetConfig();
        if (config.DolphinLocation == null || config.GameLocation == null || config.UserFolderPath == null) 
            return;
        config.ForceWiimote = value;
        ConfigManager.SaveConfigToJson();
    }
    
    
    
    public static void SaveLaunchWithDolphinWindow(bool value)
    {
        var config = ConfigManager.GetConfig();
        if (config.DolphinLocation == null || config.GameLocation == null || config.UserFolderPath == null) 
            return;
        config.LaunchWithDolphin = value;
        ConfigManager.SaveConfigToJson();
    }
}
