using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Services.Launcher;

public static class Launcher
{
    private static string RRLaunchJsonFilePath => Path.Combine(PathManager.WheelWizardAppdataPath, "RR.json");
    
    private static void KillDolphin() //dont tell PETA
    {
        var dolphinLocation = PathManager.DolphinFilePath;
        if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(dolphinLocation)).Length == 0) return;

        var dolphinProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(dolphinLocation));
        foreach (var process in dolphinProcesses)
        {
            process.Kill();
        }
    }
    
    public static void LaunchDolphin(string arguments = "", bool shellExecute = false)
    {
        var dolphinLocation = PathManager.DolphinFilePath;
        if (dolphinLocation == "")
        {
            MessageBox.Show(Phrases.PopupText_NotFindDolphin, Common.Term_Error,
                            MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = dolphinLocation,
            Arguments = arguments,
            UseShellExecute = shellExecute
        });
    }
    
    public static async Task LaunchRetroRewind()
    {
        try
        {
            KillDolphin();
            if (WiiMoteSettings.IsForceSettingsEnabled())
                WiiMoteSettings.DisableVirtualWiiMote();
            await ModsLaunchHelper.PrepareModsForLaunch();
            if (!File.Exists(PathManager.GameFilePath))
            {
                MessageBox.Show(Phrases.PopupText_NotFindGame, Common.Term_Error, 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RetroRewindLaunchHelper.GenerateLaunchJson();
            var dolphinLaunchType = (bool)SettingsManager.LAUNCH_WITH_DOLPHIN.Get() ? "" : "-b";
            LaunchDolphin(
                $"{dolphinLaunchType} -e \"{RRLaunchJsonFilePath}\" --config=Dolphin.Core.EnableCheats=False");
        }
        catch (Exception e)
        {
            // I rather not translate this message, makes it easier to check where a given error came from
            MessageBox.Show($"Failed to launch Retro Rewind: {e.Message}");
        }
    }

    public static async Task LaunchMiiChannel()
    {
        WiiMoteSettings.EnableVirtualWiiMote();
        await MiiChannelLaunchHelper.LaunchMiiChannel();
    }
}
