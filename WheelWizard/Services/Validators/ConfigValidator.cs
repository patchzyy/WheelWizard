using System;
using System.IO;
using CT_MKWII_WPF.Models;
using Newtonsoft.Json;

namespace CT_MKWII_WPF.Utilities.Configuration
{
    public static class ConfigValidator
    {
        public static bool SetupCorrectly()
        {
            var config = ConfigManager.GetConfig();
            if (config == null) return false;
            var gfxFile = Path.Combine(config.UserFolderPath, "Config", "GFX.ini");
            return File.Exists(config.DolphinLocation) &&
                   File.Exists(config.GameLocation) &&
                   File.Exists(gfxFile);
        }

        public static bool DoesConfigExist() => File.Exists(ConfigManager.GetWheelWizardAppdataPath());
        public static bool ConfigCorrectAndExists() => DoesConfigExist() && SetupCorrectly();

        public static bool IsConfigFileFinishedSettingUp()
        {
            var config = ConfigManager.GetConfig();
            return config != null && Directory.Exists(config.UserFolderPath) && File.Exists(config.DolphinLocation) &&
                   File.Exists(config.GameLocation);
        }

        public static ModData[] GetMods()
        {
            //go to the appdata
            var modConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CT-MKWII", "Mods", "modconfig.json");
            //[{"IsEnabled":true,"Title":"rocky_wii_v1p1"},{"IsEnabled":true,"Title":"sticks_rosa_-_20"},{"IsEnabled":true,"Title":"sticks_daisy"},{"IsEnabled":true,"Title":"n64_boom_boom_fortress_v1"}]
            if (!File.Exists(modConfigPath))
            {
                // MessageBox.Show("Mod config file not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ModData[0];
            }

            var json = File.ReadAllText(modConfigPath);
            return JsonConvert.DeserializeObject<ModData[]>(json);
        }

        public static void SaveWiimoteSettings(bool forceWiimote)
        {
            var config = ConfigManager.GetConfig();
            ConfigManager.SaveSettings(config.DolphinLocation, config.GameLocation, config.UserFolderPath,
                config.HasRunNandTutorial, forceWiimote);
        }
    }
}