using System.Diagnostics;
using System.Linq;

namespace WheelWizard.Services.Settings;

public static class LinuxVerifications
{
    public static bool IsValidCommand(string command)
    {
        try
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/sh", 
                Arguments = $"-c \"command -v {command.Split(' ').First()}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(processInfo);
            process.WaitForExit(3000); // Wait up to 3 seconds
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool IsDolphinInstalledInFlatpak()
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "flatpak",
                Arguments = "info org.DolphinEmu.dolphin-emu",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit(5000); // wait up to 5 seconds
                return process.ExitCode == 0;
            }
        }
        catch
        {
            return false;
        }
    }
    
    
}
