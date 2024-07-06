using System;
using System.IO;
using System.Windows;
using CT_MKWII_WPF.Pages;
using CT_MKWII_WPF.Utils;

public static class DolphinInstaller
{
    public static bool IsUserFolderValid()
    {
        var dolphinFolder = SettingsUtils.GetUserPathLocation();
        return Directory.Exists(dolphinFolder);
    }

    public static void InstallDolphin()
    {
        // Implement installation logic for Dolphin.
        MessageBox.Show("Dolphin installation logic not implemented yet.");
    }
}