using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Services.IdkWhereThisShouldGo;
using CT_MKWII_WPF.Services.Settings;
using System.IO;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Services.WiiManagement;

public static class MiiChannelManager
{
    private static string MiiChannelPath => ConfigManager.GetWheelWizardAppdataPath() + "/MiiChannel.wad";
    private static bool MiiChannelExists => File.Exists(MiiChannelPath);

    public static async Task LaunchMiiChannel()
    {
        if (!MiiChannelExists)
        {
            await DownloadHelper.DownloadToLocation(Endpoints.MarioCubeUrl, MiiChannelPath);
            //we wait to make sure the file is written to disk
            await Task.Delay(200);
        }

        DolphinSettingHelper.LaunchDolphin($"-b \"{MiiChannelPath}\"");
    }
}
