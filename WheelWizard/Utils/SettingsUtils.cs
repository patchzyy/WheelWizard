using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using CT_MKWII_WPF.Pages;

namespace CT_MKWII_WPF.Utils
{
    public static class SettingsUtils
    {
        //in appdata CT-MKWII
        private static readonly string _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/CT-MKWII", "config.json");
        private static Config _config;

        static SettingsUtils()
        {
            LoadConfigFromFile();
        }

        public static string getWheelWizardAppPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/CT-MKWII");
        }

        public static string getWheelWizardConfigPath()
        {
            return _configFilePath;
        }

        public static bool SetupCorrectly()
        {
            if (_config == null) return false;

            string gfxFile = Path.Combine(_config.UserFolderPath, "Config", "GFX.ini");
            return File.Exists(_config.DolphinLocation) && 
                   File.Exists(_config.GameLocation) && 
                   File.Exists(gfxFile);
        }

        public static bool doesConfigExist()
        {
            return File.Exists(_configFilePath);
        }
        
        public static bool configCorrectAndExists()
        {
            return doesConfigExist() && SetupCorrectly();
        }
        
        
        public static void SaveSettings(string dolphinPath, string gamePath, string userFolderPath)
        {
            // Update the _config object with the new values
            _config.DolphinLocation = dolphinPath;
            _config.GameLocation = gamePath;
            _config.UserFolderPath = userFolderPath;
            // Serialize the _config object to JSON and save it to the config file
            var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath));
            try
            {
                File.WriteAllText(_configFilePath, configJson);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred while saving settings. Please try again later. \n \nError: " + e.Message);
                throw;
            }
        }
        public static void SaveSettings(string dolphinPath, string gamePath, string userFolderPath, bool hasRunNANDTutorial)
        {
            // Update the _config object with the new values
            _config.DolphinLocation = dolphinPath;
            _config.GameLocation = gamePath;
            _config.UserFolderPath = userFolderPath;
            _config.HasRunNANDTutorial = hasRunNANDTutorial;
            // Serialize the _config object to JSON and save it to the config file
            var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(_configFilePath, configJson);
        }

        private static void LoadConfigFromFile()
        {
            if (File.Exists(_configFilePath))
            {
                _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(_configFilePath));
            }
            else
            {
                _config = new Config();
            }
        }

        public static string GetConfigText()
        {
            return File.Exists(_configFilePath) ? File.ReadAllText(_configFilePath) : string.Empty;
        }
        
        public static void setHasSeenNandPopUp(bool hasRunNANDTutorial)
        {
            _config.HasRunNANDTutorial = hasRunNANDTutorial;
            SaveSettings(_config.DolphinLocation, _config.GameLocation, _config.UserFolderPath, hasRunNANDTutorial);
        }

        public static string GetGameLocation() => _config.GameLocation;

        public static string GetDolphinLocation() => _config.DolphinLocation;

        public static string GetUserPathLocation() => _config.UserFolderPath;

        public static string GetLoadPathLocation()
        {
            return Path.Combine(_config.UserFolderPath, "Load");
        }

        public static bool HasRunNANDTutorial() => _config.HasRunNANDTutorial;

        public static bool IsConfigFileFinishedSettingUp()
        {
            if (_config == null || !Directory.Exists(_config.UserFolderPath) || !File.Exists(_config.DolphinLocation) || !File.Exists(_config.GameLocation))
            {
                return false;
            }
            return true;
        }

        public static string FindGFXFile()
        {

            var folderPath = GetUserPathLocation();
            var configFolder = Path.Combine(folderPath, "Config");
            var gfxFile = Path.Combine(configFolder, "GFX.ini");

            if (File.Exists(gfxFile))
            {
                return gfxFile;
            }

            MessageBox.Show($"Could not find GFX file, tried looking in {gfxFile}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return string.Empty;
        }

        private class Config
        {
            public string DolphinLocation { get; set; }
            public string GameLocation { get; set; }
            public string UserFolderPath { get; set; }
            public bool HasRunNANDTutorial { get; set; }
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
    }
}