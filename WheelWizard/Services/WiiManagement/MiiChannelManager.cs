using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Services.Configuration;
using CT_MKWII_WPF.Services.WiiManagement.DolphinHelpers;
using CT_MKWII_WPF.Views;

namespace CT_MKWII_WPF.Services.WiiManagement;

public static class MiiChannelManager
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
        await DownloadHelper.DownloadFileWithWindow(Endpoints.MarioCubeUrl, GetSavedChannelLocation(), progressWindow);
        progressWindow.Close();
    }
}