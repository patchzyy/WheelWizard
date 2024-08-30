using CT_MKWII_WPF.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace CT_MKWII_WPF.Services.Settings;

public static class ConfigValidator
{

    public static void SaveWiimoteSettings(bool value)
    {
        if (SettingsHelper.PathsSetupCorrectly()) 
            return;
        
        var config = ConfigManager.GetConfig();
        config.ForceWiimote = value;
        ConfigManager.SaveConfigToJson();
    }
    
    public static void SaveLaunchWithDolphinWindow(bool value)
    {
        if (SettingsHelper.PathsSetupCorrectly()) 
            return;
        
        var config = ConfigManager.GetConfig();
        config.LaunchWithDolphin = value;
        ConfigManager.SaveConfigToJson();
    }
}
