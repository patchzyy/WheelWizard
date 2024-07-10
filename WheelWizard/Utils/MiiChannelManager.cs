using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Pages;
using CT_MKWII_WPF.Utils.DolphinHelpers;

namespace CT_MKWII_WPF.Utils;

public class MiiChannelManager
{
    public static string getSavedChannelLocation()
    {
        return SettingsUtils.getWheelWizardAppPath() + "/MiiChannel.wad";
    }

    public async static Task LaunchMiiChannel()
    {
        //if mii channel isnt downloaded yet, download it
        if (!MiiChannelExists())
        {
            await DownloadMiiChannel();
            //todo: maybe remove?
            await Task.Delay(200);
        }
        //launch mii channel
        DolphinSettingHelper.LaunchDolphin($"-b \"{getSavedChannelLocation()}\"");
    }

    private static bool MiiChannelExists()
    {
        return File.Exists(getSavedChannelLocation());
    }

    public async static Task DownloadMiiChannel()
    {
        try
        {
            var progressWindow = new ProgressWindow();
            progressWindow.Show();
            await DownloadUtils.DownloadFileWithWindow("https://repo.mariocube.com/WADs/Other/Mii%20Channel%20Symbols%20-%20HACS.wad", getSavedChannelLocation(), progressWindow);
            progressWindow.Close();
        }
        catch (Exception e)
        {
            MessageBox.Show("An error occurred while downloading the Mii Channel. Please try again later. \n \nError: " + e.Message);
        }
    }
}