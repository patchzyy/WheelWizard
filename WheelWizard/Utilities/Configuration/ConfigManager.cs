using System;
using System.IO;
using System.Windows;
using CT_MKWII_WPF.Models;
using Newtonsoft.Json;

namespace CT_MKWII_WPF.Utilities.Configuration;

public static class ConfigManager
{
    private static readonly string WheelWizardAppdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "config.json");
    private static Config _config;
    static ConfigManager()
    {
        LoadConfigFromFile();
    }
    
    public static Config GetConfig() => _config;
    public static string GetWheelWizardAppdataPath() =>  WheelWizardAppdataPath;
    
    public static void SaveSettings(string dolphinPath, string gamePath, string userFolderPath, bool hasRunNandTutorial, bool forceDisableWiimote)
    {
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
        Directory.CreateDirectory(Path.GetDirectoryName(WheelWizardAppdataPath));
        try
        {
            File.WriteAllText(WheelWizardAppdataPath, configJson);
        }
        catch (Exception e)
        {
            MessageBox.Show("An error occurred while saving settings. \n \nError: " + e.Message);
            throw;
        }
    }
    
    private static void LoadConfigFromFile()
    {
        if (File.Exists(WheelWizardAppdataPath))
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(WheelWizardAppdataPath));
        else
            _config = new Config { ForceWiimote = true };
    }
}
