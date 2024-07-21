using System;
using System.IO;
using System.Windows;
using CT_MKWII_WPF.Models;
using Newtonsoft.Json;

namespace CT_MKWII_WPF.Services.Configuration;

public static class ConfigManager
{
    private static readonly string WheelWizardConfigFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "config.json");
    
    private static readonly string WheelWizardAppdataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII");

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
    public static string GetWheelWizardAppdataPath() => WheelWizardAppdataPath;
    
    public static string GetWheelWizardConfigPath() => WheelWizardConfigFilePath;

    public static void SaveSettings(string dolphinPath, string gamePath, string userFolderPath, bool hasRunNandTutorial,
        bool forceDisableWiimote)
    {
        if (_config == null) return;
        _config.DolphinLocation = dolphinPath;
        _config.GameLocation = gamePath;
        _config.UserFolderPath = userFolderPath;
        _config.HasRunNandTutorial = hasRunNandTutorial;
        _config.ForceWiimote = forceDisableWiimote;
        SaveConfigToJson();
    }

    private static void SaveConfigToJson()
    {
        var configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
        Directory.CreateDirectory(Path.GetDirectoryName(WheelWizardConfigFilePath)!);
        try
        {
            File.WriteAllText(WheelWizardConfigFilePath, configJson);
        }
        catch (Exception e)
        {
            MessageBox.Show("An error occurred while saving settings. \n \nError: " + e.Message);
            throw;
        }
    }

    private static void LoadConfigFromFile()
    {
        if (File.Exists(WheelWizardConfigFilePath))
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(WheelWizardConfigFilePath));
        else
            _config = new Config { ForceWiimote = true };
    }
}