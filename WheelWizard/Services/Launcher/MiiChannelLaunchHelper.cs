using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Views.Popups;
using System.IO;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Services.Launcher;

public static class MiiChannelLaunchHelper
{
    private static string MiiChannelPath => Path.Combine(PathManager.WheelWizardAppdataPath, "MiiChannel.wad");
    
    public static async Task LaunchMiiChannel()
    {
        var miiChannelExists = File.Exists(MiiChannelPath);;
        
        if (!miiChannelExists)
        {
            var downloadQuestion = new YesNoWindow()
                                .SetMainText("Install MiiChannel?")
                                .SetExtraText("Do you want to install the MiiChannel to launch it?");
            
            if (downloadQuestion.AwaitAnswer())
            {
                miiChannelExists = true;
                await DownloadHelper.DownloadToLocation(Endpoints.MiiChannelWAD, MiiChannelPath);
                //we wait to make sure the file is written to disk
                await Task.Delay(200);
            }
        }

        if(miiChannelExists)
            Launcher.LaunchDolphin($"-b \"{MiiChannelPath}\"");
    }
}
