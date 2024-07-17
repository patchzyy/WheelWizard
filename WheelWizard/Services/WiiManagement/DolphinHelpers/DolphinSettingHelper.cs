using System;
using System.IO;
using System.Linq;
using System.Windows;
using CT_MKWII_WPF.Utilities.Configuration;

namespace CT_MKWII_WPF.Services.WiiManagement.DolphinHelpers;

public class DolphinSettingHelper
{
    public static string ReadIniSetting(string fileLocation, string section, string settingToRead)
    {
        if (!File.Exists(fileLocation))
        {
            MessageBox.Show("Something went wrong, INI file could not be read, Message Patchzy with the following error: " + fileLocation, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        //read through every line to find the section
        var lines = File.ReadAllLines(fileLocation);
        var sectionFound = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == $"[{section}]")
            {
                sectionFound = true;
                break;
            }
        }
        if (!sectionFound)
        {
            // MessageBox.Show("Could not find section in INI file, Message Patchzy with the following error: " + Section, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return "";
        }
        //now we know the section exists, we need to find the setting
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(settingToRead))
            {
                //we found the setting, now we need to return the value
                var setting = lines[i].Split("=");
                return setting[1].Trim();
            }
        }
        // MessageBox.Show("Could not find setting in INI file, Message Patchzy with the following error: " + SettingToRead, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return "";
    }
    
    public static string AutomaticallyFindDolphinPath()
    {
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator");
        if (Directory.Exists(appDataPath))
            return appDataPath;

        string documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Dolphin Emulator");
        if (Directory.Exists(documentsPath))
            return documentsPath;
        return string.Empty;
    }
    
    public static string ReadIniSetting(string fileLocation, string settingToRead)
    {
        if (!File.Exists(fileLocation))
        {
            MessageBox.Show("Something went wrong, INI file could not be read, Message Patchzy with the following error: " + fileLocation, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        var lines = File.ReadAllLines(fileLocation);
        foreach (string line in lines)
        {
            if (line.StartsWith(settingToRead))
            {
                var setting = line.Split("=");
                return setting[1].Trim();
            }
        }
        return "";
    }
    public static void ChangeIniSettings(string fileLocation, string section, string settingToChange, string value)
    {
        if (!File.Exists(fileLocation))
        {
            MessageBox.Show($"Something went wrong, INI file could not be found, Message Patchzy with the following error: {fileLocation}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var lines = File.ReadAllLines(fileLocation).ToList();
        bool sectionFound = false;
        bool settingFound = false;
        int sectionIndex = -1;

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Trim() == $"[{section}]")
            {
                sectionFound = true;
                sectionIndex = i;
            }
            else if (sectionFound && (lines[i].StartsWith($"{settingToChange}=") || lines[i].StartsWith($"{settingToChange} =")))
            {
                lines[i] = $"{settingToChange} = {value}";
                settingFound = true;
                break;
            }
            else if (lines[i].Trim().StartsWith("[") && lines[i].Trim().EndsWith("]") && sectionFound)
            {
                break;
            }
        }

        if (!sectionFound)
        {
            lines.Add($"[{section}]");
            lines.Add($"{settingToChange}={value}");
        }
        else if (!settingFound)
        {
            lines.Insert(sectionIndex + 1, $"{settingToChange}={value}");
        }

        File.WriteAllLines(fileLocation, lines);
    }
    
    public static void LaunchDolphin(string arguments = "")
    {
        var dolphinLocation = PathManager.GetDolphinLocation();
        if (dolphinLocation == "")
        {
            MessageBox.Show("Could not find Dolphin Emulator, please set the path in settings", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        System.Diagnostics.Process.Start(dolphinLocation, arguments);
    }
}