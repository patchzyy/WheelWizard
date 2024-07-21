using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CT_MKWII_WPF.Services.Configuration;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Validators;
using CT_MKWII_WPF.Services.WiiManagement;

namespace CT_MKWII_WPF.Services.Launcher;

public static class RetroRewindLauncher
{
    public static void PlayRetroRewind(bool playTt)
    {
        KillDolphin();
        if (WiiMoteSettings.IsForceSettingsEnabled()) WiiMoteSettings.DisableVirtualWiiMote();
        PrepareModsForLaunch();
        
        var dolphinLocation = PathManager.GetDolphinLocation();
        var gamePath = PathManager.GetGameLocation();
        LaunchJsonGenerator.GenerateLaunchJson(playTt);
        var launchJson = Path.Combine(ConfigManager.GetWheelWizardAppdataPath(), "RR.json");

        if (!File.Exists(dolphinLocation) || !File.Exists(gamePath))
        {
            MessageBox.Show("Message patchzy with the following error: " + dolphinLocation + " " + gamePath, 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = dolphinLocation,
                Arguments = $"-e \"{launchJson}\"",
                UseShellExecute = false
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MessageBox.Show("Failed to start Dolphin Emulator. Please try again.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    
    private static void KillDolphin()
    {
        var dolphinLocation = PathManager.GetDolphinLocation();
        if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(dolphinLocation)).Length != 0)
        {
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(dolphinLocation)))
            {
                process.Kill();
            }
        }
    }

    private static void PrepareModsForLaunch()
    {
        var mods = ConfigValidator.GetMods();
        if (mods.Length == 0) return;
        Array.Reverse(mods);
        var myStuffFolder = Path.Combine(PathManager.GetLoadPathLocation(), "Riivolution", "RetroRewind6",
            "MyStuff");
        if (Directory.Exists(myStuffFolder))
        {
            Directory.Delete(myStuffFolder, true);
        }
        foreach (var mod in mods)
        {
            if (mod.IsEnabled)
            {
                DirectoryHandler.InstallMod(mod);
            }
        }
    }
}