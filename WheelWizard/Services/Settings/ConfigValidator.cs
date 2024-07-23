﻿using CT_MKWII_WPF.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace CT_MKWII_WPF.Services.Settings
{
    public static class ConfigValidator
    {
        public static bool SetupCorrectly()
        {
            var config = ConfigManager.GetConfig();
            if (config.UserFolderPath == null) return false;
            var gfxFile = Path.Combine(config.UserFolderPath, "Config", "GFX.ini");
            return File.Exists(config.DolphinLocation) &&
                   File.Exists(config.GameLocation) &&
                   File.Exists(gfxFile);
        }

        private static bool DoesConfigExist() => File.Exists(ConfigManager.WheelWizardConfigFilePath);
        public static bool ConfigCorrectAndExists() => DoesConfigExist() && SetupCorrectly();

        public static bool IsConfigFileFinishedSettingUp()
        {
            var config = ConfigManager.GetConfig();
            return Directory.Exists(config.UserFolderPath) && File.Exists(config.DolphinLocation) &&
                   File.Exists(config.GameLocation);
        }

        public static ModData[] GetMods()
        {
            var modConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CT-MKWII", "Mods", "modconfig.json");

            if (!File.Exists(modConfigPath))
            {
                return Array.Empty<ModData>();
            }

            var json = File.ReadAllText(modConfigPath);
            var mods = JsonConvert.DeserializeObject<ModData[]>(json);
            return mods ?? Array.Empty<ModData>();
        }

        public static void SaveWiimoteSettings(bool forceWiimote)
        {
            var config = ConfigManager.GetConfig();
            if (config.DolphinLocation == null || config.GameLocation == null || config.UserFolderPath == null) return;
            ConfigManager.SaveSettings(config.DolphinLocation, config.GameLocation, config.UserFolderPath,
                config.HasRunNandTutorial, forceWiimote);
        }
    }
}