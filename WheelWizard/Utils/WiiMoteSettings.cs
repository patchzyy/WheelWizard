using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Shapes;

namespace CT_MKWII_WPF.Utils;

public class WiiMoteSettings
{
    private const string WiimoteSection = "[Wiimote1]";
    private const string SourceParameter = "Source";

    public static string GetSavedWiiMoteLocation()
    {
        return SettingsUtils.FindWiiMoteNew();
    }

    public static void EnableVirtualWiiMote()
    {
        ModifyWiiMoteSource(1);
    }
    
    public static void DisableVirtualWiiMote()
    {
        ModifyWiiMoteSource(0);
    }
    
    public static bool IsForceSettingsEnabled()
    {
        return SettingsUtils.GetForceWiimote();
    }
    
    private static void ModifyWiiMoteSource(int sourceValue)
    {
        string configPath = GetSavedWiiMoteLocation();
        if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
        {
            throw new FileNotFoundException("WiiMote configuration file not found.");
        }

        string[] lines = File.ReadAllLines(configPath);
        bool inWiimote1Section = false;
        bool sourceModified = false;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim() == WiimoteSection)
            {
                inWiimote1Section = true;
                continue;
            }

            if (inWiimote1Section)
            {
                if (lines[i].Trim().StartsWith("[")) // New section started
                {
                    break;
                }

                if (lines[i].Trim().StartsWith($"{SourceParameter} ="))
                {
                    lines[i] = $"{SourceParameter} = {sourceValue}";
                    sourceModified = true;
                    break;
                }
            }
        }

        if (!sourceModified)
        {
            // Source parameter not found, add it to the Wiimote1 section
            int insertIndex = Array.FindIndex(lines, l => l.Trim() == WiimoteSection) + 1;
            Array.Resize(ref lines, lines.Length + 1);
            Array.Copy(lines, insertIndex, lines, insertIndex + 1, lines.Length - insertIndex - 1);
            lines[insertIndex] = $"{SourceParameter} = {sourceValue}";
        }

        File.WriteAllLines(configPath, lines);
    }
    
}
