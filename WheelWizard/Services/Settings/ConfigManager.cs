using CT_MKWII_WPF.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace CT_MKWII_WPF.Services.Settings;

public static class ConfigManager
{
    private static Config? _config;

    static ConfigManager()
    {
        LoadConfigFromFile();
    }

    public static Config GetConfig()
    {
        if (_config == null)
            throw new Exception("Config is null, this should never happen");
        return _config;
    }

    public static void SaveSettings(string dolphinPath, string gamePath, string userFolderPath, bool hasRunNandTutorial,
        bool forceDisableWiimote, bool launchWithDolphin)
    {
        if (_config == null) return;
        _config.DolphinLocation = dolphinPath;
        _config.GameLocation = gamePath;
        _config.UserFolderPath = userFolderPath;
        _config.HasRunNandTutorial = hasRunNandTutorial;
        _config.ForceWiimote = forceDisableWiimote;
        _config.LaunchWithDolphin = launchWithDolphin;
        SaveConfigToJson();
    }

    public static void SaveConfigToJson()
    {
        var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
        Directory.CreateDirectory(Path.GetDirectoryName(PathManager.WheelWizardConfigFilePath)!);
        try
        {
            File.WriteAllText(PathManager.WheelWizardConfigFilePath, configJson);
        }
        catch (Exception e)
        {
            MessageBox.Show("An error occurred while saving settings. \n \nError: " + e.Message);
            throw;
        }
    }

    private static void LoadConfigFromFile()
    {
        if (File.Exists(PathManager.WheelWizardConfigFilePath))
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(PathManager.WheelWizardConfigFilePath));
        else
            _config = new Config { ForceWiimote = true };
    }
}
