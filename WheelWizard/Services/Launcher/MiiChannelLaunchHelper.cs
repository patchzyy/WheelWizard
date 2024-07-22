using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Views.Popups;
using System.IO;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Services.Launcher;

public static class MiiChannelLaunchHelper
{
    private static string MiiChannelPath => Path.Combine(ConfigManager.GetWheelWizardAppdataPath(), "MiiChannel.wad");
    
    public static async Task LaunchMiiChannel()
    {
        var miiChannelExists = File.Exists(MiiChannelPath);;
        
        if (!miiChannelExists)
        {
            var adminResult = YesNoMessagebox.Show(
                "You dont have the MiiChannel installed, do you want to install it?", 
                "Yes", "No");
            if (adminResult)
            {
                miiChannelExists = true;
                await DownloadHelper.DownloadToLocation(Endpoints.MarioCubeUrl, MiiChannelPath);
                //we wait to make sure the file is written to disk
                await Task.Delay(200);
            }
        }

        if(miiChannelExists)
            Launcher.LaunchDolphin($"-b \"{MiiChannelPath}\"");
    }
}
