using System;
using System.Diagnostics;
using System.IO;
using WheelWizard.Views.Popups.Generic;

namespace WheelWizard.Services.Launcher.Helpers;

public static class DolphinLaunchHelper
{
    public static void KillDolphin() //dont tell PETA
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
                             "Please make sure the settings are set up correctly.")
                .Show();
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
                .SetInfoText($"Reason: {ex.Message}")
                .Show();
        }
    }
}
