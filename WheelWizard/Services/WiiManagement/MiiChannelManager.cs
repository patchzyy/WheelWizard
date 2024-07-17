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
    private static string GetSavedChannelLocation() => ConfigManager.GetWheelWizardAppdataPath() + "/MiiChannel.wad";
    private static bool MiiChannelExists() => File.Exists(GetSavedChannelLocation());

    public static async Task LaunchMiiChannel()
    {
        if (!MiiChannelExists())
        {
            await DownloadMiiChannel();
            //we wait to make sure the file is written to disk
            await Task.Delay(200);
        }

        DolphinSettingHelper.LaunchDolphin($"-b \"{GetSavedChannelLocation()}\"");
    }

    private static async Task DownloadMiiChannel()
    {
        var progressWindow = new ProgressWindow();
        progressWindow.Show();
        await DownloadUtils.DownloadFileWithWindow(
            "https://repo.mariocube.com/WADs/Other/Mii%20Channel%20Symbols%20-%20HACS.wad", GetSavedChannelLocation(),
            progressWindow);
        progressWindow.Close();
    }
}