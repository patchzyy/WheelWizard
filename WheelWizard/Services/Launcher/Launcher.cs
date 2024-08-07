using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement;
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
            MessageBox.Show("Could not find Dolphin Emulator, please set the path in settings",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = dolphinLocation,
            Arguments = arguments,
            UseShellExecute = shellExecute
        });
    }
    
    public static async Task LaunchRetroRewind(bool playTt)
    {
        KillDolphin();
        if (WiiMoteSettings.IsForceSettingsEnabled()) 
            WiiMoteSettings.DisableVirtualWiiMote();
        await ModsLaunchHelper.PrepareModsForLaunch();
        if (!File.Exists(PathManager.GameFilePath))
        {
            MessageBox.Show("Could not find the game, please set the path in settings",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        RetroRewindLaunchHelper.GenerateLaunchJson(playTt);
        var dolphinLaunchType = ConfigManager.GetConfig().LaunchWithDolphin ? "" : "-b";
        LaunchDolphin( $"{dolphinLaunchType} -e \"{RRLaunchJsonFilePath}\" --config=Dolphin.Core.EnableCheats=False");
    }

    public static async Task LaunchMiiChannel()
    {
        WiiMoteSettings.EnableVirtualWiiMote();
        await MiiChannelLaunchHelper.LaunchMiiChannel();
    }
}
