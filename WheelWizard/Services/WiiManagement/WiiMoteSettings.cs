using CT_MKWII_WPF.Services.Settings;
using System;
using System.IO;

namespace CT_MKWII_WPF.Services.WiiManagement;

public static class WiiMoteSettings
{
    private const string WiimoteSection = "[Wiimote1]";
    private const string SourceParameter = "Source";

    private static string GetSavedWiiMoteLocation() => DolphinSettingHelper.FindWiiMoteNew();
    public static void EnableVirtualWiiMote() => ModifyWiiMoteSource(1);
    public static void DisableVirtualWiiMote() => ModifyWiiMoteSource(0);
    public static bool IsForceSettingsEnabled() => ConfigManager.GetConfig().ForceWiimote;

    private static void ModifyWiiMoteSource(int sourceValue)
    {
        var configPath = GetSavedWiiMoteLocation();
        if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            throw new FileNotFoundException("WiiMote configuration file not found.");

        var lines = File.ReadAllLines(configPath);
        var inWiimote1Section = false;
        var sourceModified = false;

        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim() == WiimoteSection)
            {
                inWiimote1Section = true;
                continue;
            }

            if (!inWiimote1Section) continue;
            if (lines[i].Trim().StartsWith("[")) // New section started
                break;

            if (!lines[i].Trim().StartsWith($"{SourceParameter} =")) continue;
            lines[i] = $"{SourceParameter} = {sourceValue}";
            sourceModified = true;
            break;
        }

        if (!sourceModified)
        {
            // Source parameter not found, add it to the Wiimote1 section
            var insertIndex = Array.FindIndex(lines, l => l.Trim() == WiimoteSection) + 1;
            Array.Resize(ref lines, lines.Length + 1);
            Array.Copy(lines, insertIndex, lines, insertIndex + 1, lines.Length - insertIndex - 1);
            lines[insertIndex] = $"{SourceParameter} = {sourceValue}";
        }

        File.WriteAllLines(configPath, lines);
    }
}
