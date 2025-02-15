using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.Launcher.Helpers;
using WheelWizard.Services.Settings;
using WheelWizard.Services.WiiManagement;
using WheelWizard.Views.Popups.Generic;

namespace WheelWizard.Services.Launcher;

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
        if (string.IsNullOrWhiteSpace(dolphinLocation) || !File.Exists(dolphinLocation))
        {
            new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Warning)
                .SetTitleText("Dolphin not found")
                .SetInfoText($"Dolphin not found at: {dolphinLocation} \n" +
                             "Please make sure the settings are set up correctly.").Show();
            return;
        }

        try
        {
            var startInfo = new ProcessStartInfo();

            // Handle Flatpak Dolphin
            if (dolphinLocation.Contains("flatpak"))
            {
                startInfo.FileName = "flatpak";
                startInfo.Arguments = $"run org.DolphinEmu.dolphin-emu {arguments}";
                startInfo.UseShellExecute = false;
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // For non-Flatpak Linux/macOS builds
                startInfo.FileName = "/bin/bash";
                startInfo.Arguments = $"-c \"{dolphinLocation} {arguments}\"";
                startInfo.UseShellExecute = false;
            }
            else
            {
                // Windows builds
                startInfo.FileName = dolphinLocation;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = shellExecute;
            }

            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Error)
                .SetTitleText("Failed to launch Dolphin")
                .SetInfoText($"Reason: {ex.Message}").Show();
        }
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
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    new MessageBoxWindow()
                        .SetMessageType(MessageBoxWindow.MessageType.Warning)
                        .SetTitleText("Invalid game path")
                        .SetInfoText(Phrases.PopupText_NotFindGame).Show();
                });
                return;
            }
            
            RetroRewindLaunchHelper.GenerateLaunchJson();
            var dolphinLaunchType = (bool)SettingsManager.LAUNCH_WITH_DOLPHIN.Get() ? "" : "-b";
            LaunchDolphin(
                $"{dolphinLaunchType} -e \"{RRLaunchJsonFilePath}\" --config=Dolphin.Core.EnableCheats=False");
        }
        catch (Exception ex)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                new MessageBoxWindow()
                    .SetMessageType(MessageBoxWindow.MessageType.Error)
                    .SetTitleText("Failed to launch Retro Rewind")
                    .SetInfoText($"Reason: {ex.Message}").Show();
            });
        }
    }

    public static async Task LaunchMiiChannel()
    {
        WiiMoteSettings.EnableVirtualWiiMote();
        await MiiChannelLaunchHelper.LaunchMiiChannel();
    }
}
