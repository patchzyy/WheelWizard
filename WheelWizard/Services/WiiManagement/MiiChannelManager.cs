using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Services.WiiManagement.DolphinHelpers;
using CT_MKWII_WPF.Utilities.Configuration;
using CT_MKWII_WPF.Utilities.Downloads;
using CT_MKWII_WPF.Views;

namespace CT_MKWII_WPF.Services.WiiManagement;

public class MiiChannelManager
{
    public static string GetSavedChannelLocation()
    {
        return ConfigManager.GetWheelWizardAppdataPath() + "/MiiChannel.wad";
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
        DolphinSettingHelper.LaunchDolphin($"-b \"{GetSavedChannelLocation()}\"");
    }

    private static bool MiiChannelExists()
    {
        return File.Exists(GetSavedChannelLocation());
    }

    public async static Task DownloadMiiChannel()
    {
        try
        {
            var progressWindow = new ProgressWindow();
            progressWindow.Show();
            await DownloadUtils.DownloadFileWithWindow("https://repo.mariocube.com/WADs/Other/Mii%20Channel%20Symbols%20-%20HACS.wad", GetSavedChannelLocation(), progressWindow);
            progressWindow.Close();
        }
        catch (Exception e)
        {
            MessageBox.Show("An error occurred while downloading the Mii Channel. Please try again later. \n \nError: " + e.Message);
        }
    }
}