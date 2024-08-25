using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CT_MKWII_WPF.Services.Settings;

public class DolphinSettingManager
{
    private static string ConfigFolderPath(string fileName) => Path.Combine(PathManager.ConfigFolderPath, fileName);
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
        if (_loaded) 
            return;
        
        _loaded = true;
    }
    
    private static string[]? ReadIniFile(string fileName)
    {
        var filePath = ConfigFolderPath(fileName);
        var lines =  FileHelper.ReadAllLinesSafe(filePath);
            
        // if (lines == null){
        //     MessageBox.Show(
        //         "Something went wrong, INI file could not be read, Plzz report this as a bug: " +
        //         filePath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        // }
        
        return lines;
    }
    
    private static string? ReadIniSetting(string fileName, string section, string settingToRead)
    {
        var lines = ReadIniFile(fileName);
        if (lines == null) 
            return null;
     
        var sectionIndex = Array.IndexOf(lines, $"[{section}]");
        if (sectionIndex == -1)
            return null;
        
        // find all the settings related to this section, we dont want to read/influence other sections
        var nextSectionName = lines.Skip(sectionIndex + 1)
                                   .FirstOrDefault(x => x.Trim().StartsWith("[") && x.Trim().EndsWith("]"));
        var nextSectionIndex = Array.IndexOf(lines, nextSectionName);
        var sectionLines = lines.Skip(sectionIndex + 1);
        if (nextSectionIndex != -1)
            sectionLines = sectionLines.Take(nextSectionIndex - sectionIndex - 1);
        
        // finally we can read the setting
        foreach (var line in sectionLines)
        {
            if (!line.Contains(settingToRead)) 
                continue;
            //we found the setting, now we need to return the value
            var setting = line.Split("=");
            return setting[1].Trim();
        }

        return null;
    }
    
    // TODO: find out when to use `setting=value` and when to use `setting = value`
    private static void ChangeIniSettings(string fileName, string section, string settingToChange, string value)
    {
        var lines = ReadIniFile(fileName)?.ToList();
        if (lines == null)
            return;
        
        var sectionIndex = lines.IndexOf($"[{section}]");
        if (sectionIndex == -1)
        {
            lines.Add($"[{section}]");
            lines.Add($"{settingToChange} = {value}");
            FileHelper.WriteAllLines(ConfigFolderPath(fileName), lines);
            return;
        }

        for (var i = sectionIndex+1; i < lines.Count; i++)
        {
            //
            if (lines[i].Trim().StartsWith("[") && lines[i].Trim().EndsWith("]"))
                break; // Setting was not found in this section, so we have to append it to the section

            if ((!lines[i].StartsWith($"{settingToChange}=") && !lines[i].StartsWith($"{settingToChange} =")))
                continue;
            
            lines[i] = $"{settingToChange} = {value}";
            FileHelper.WriteAllLines(ConfigFolderPath(fileName), lines);
            return;
        }
        // you only get here if the setting was not found in the section
        
        lines.Insert(sectionIndex + 1, $"{settingToChange} = {value}");
        FileHelper.WriteAllLines(ConfigFolderPath(fileName), lines);
    }
}
