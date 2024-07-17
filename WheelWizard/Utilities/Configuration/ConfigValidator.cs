using System;
using System.IO;
using System.Windows;
using CT_MKWII_WPF.Models;
using Newtonsoft.Json;
using CT_MKWII_WPF.Pages;

namespace CT_MKWII_WPF.Utils
{
    public static class ConfigValidator
    {
        public static bool SetupCorrectly()
        {
            Config config = ConfigManager.GetConfig();
            if (config == null) return false;
            string gfxFile = Path.Combine(config.UserFolderPath, "Config", "GFX.ini");
            return File.Exists(config.DolphinLocation) && 
                   File.Exists(config.GameLocation) && 
                   File.Exists(gfxFile);
        }

        public static bool doesConfigExist()
        {
            return File.Exists(ConfigManager.GetWheelWizardAppdataPath());
        }
        
        public static bool configCorrectAndExists()
        {
            return doesConfigExist() && SetupCorrectly();
        }
        
        

        public static string GetLoadPathLocation()
        {
            return Path.Combine(ConfigManager.GetConfig().UserFolderPath, "Load");
        }
        

        public static bool IsConfigFileFinishedSettingUp()
        {
            Config _config = ConfigManager.GetConfig();
            if (_config == null || !Directory.Exists(_config.UserFolderPath) || !File.Exists(_config.DolphinLocation) || !File.Exists(_config.GameLocation))
            {
                return false;
            }
            return true;
        }
        
        

        public static ModData[] GetMods()
        {
            //go to the appdata
            var modConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", "modconfig.json");
            //[{"IsEnabled":true,"Title":"rocky_wii_v1p1"},{"IsEnabled":true,"Title":"sticks_rosa_-_20"},{"IsEnabled":true,"Title":"sticks_daisy"},{"IsEnabled":true,"Title":"n64_boom_boom_fortress_v1"}]
            if (!File.Exists(modConfigPath))
            {
                // MessageBox.Show("Mod config file not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ModData[0];
            }
            var json = File.ReadAllText(modConfigPath);
            return JsonConvert.DeserializeObject<ModData[]>(json);
        }

        public static void SaveWiimoteSettings(bool ForceWiimote)
        {
            Config _config = ConfigManager.GetConfig();
            ConfigManager.SaveSettings(_config.DolphinLocation, _config.GameLocation, _config.UserFolderPath, _config.HasRunNANDTutorial, ForceWiimote);
        }
    }
}