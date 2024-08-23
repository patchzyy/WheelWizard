using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;
using System.Collections.Generic;
using System.Text.Json;
using JsonElement = System.Text.Json.JsonElement;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace CT_MKWII_WPF.Services.Settings;

public class DolphinSettingManager
{
    private static string ConfigFolderPath => PathManager.ConfigFolderPath;
    private bool _loaded;
    private readonly Dictionary<string, DolphinSetting> _settings = new();
    
    public static DolphinSettingManager Instance { get; } = new();
    private DolphinSettingManager() { }
    
    public void RegisterSetting(DolphinSetting setting)
    {
        if (_loaded) 
            return;
        
        _settings.Add(setting.Name, setting);
    }
    
    public void SaveSettings(DolphinSetting invokingSetting)
    {
  
    }
    
    public void LoadSettings()
    {
    
    }
    
    /*
    private static string ReadIniSetting(string fileLocation, string section, string settingToRead)
    {
        if (!FileHelper.FileExists(fileLocation))
        {
            MessageBox.Show(
                "Something went wrong, INI file could not be read, Plzz report this as a bug: " +
                fileLocation, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //read through every line to find the section
        var lines = File.ReadAllLines(fileLocation);
        var sectionFound = lines.Any(t => t == $"[{section}]");

        if (!sectionFound) 
            return "";

        //now we know the section exists, we need to find the setting
        foreach (var line in lines)
        {
            if (!line.Contains(settingToRead)) 
                continue;
            //we found the setting, now we need to return the value
            var setting = line.Split("=");
            return setting[1].Trim();
        }

        return "";
    }
    
    private static string ReadIniSetting(string fileLocation, string settingToRead)
    {
        if (!File.Exists(fileLocation))
        {
            MessageBox.Show(
                "Something went wrong, INI file could not be read, Plzz report this as a bug: " +
                fileLocation, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        var lines = File.ReadAllLines(fileLocation);
        foreach (var line in lines)
        {
            if (!line.StartsWith(settingToRead)) 
                continue;
            
            var setting = line.Split("=");
            return setting[1].Trim();
        }

        return "";
    }

    private static void ChangeIniSettings(string fileLocation, string section, string settingToChange, string value)
    {
        if (!File.Exists(fileLocation))
        {
            MessageBox.Show(
                $"Something went wrong, INI file could not be found, Plzz report this as a bug: {fileLocation}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var lines = File.ReadAllLines(fileLocation).ToList();
        var sectionFound = false;
        var settingFound = false;
        var sectionIndex = -1;

        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Trim() == $"[{section}]")
            {
                sectionFound = true;
                sectionIndex = i;
            }
            else if (sectionFound && (lines[i].StartsWith($"{settingToChange}=") ||
                                      lines[i].StartsWith($"{settingToChange} =")))
            {
                lines[i] = $"{settingToChange} = {value}";
                settingFound = true;
                break;
            }
            else if (lines[i].Trim().StartsWith("[") && lines[i].Trim().EndsWith("]") && sectionFound)
                break;
        }

        if (!sectionFound)
        {
            lines.Add($"[{section}]");
            lines.Add($"{settingToChange}={value}");
        }
        else if (!settingFound)
            lines.Insert(sectionIndex + 1, $"{settingToChange}={value}");
        
        File.WriteAllLines(fileLocation, lines);
    }
    */
}
