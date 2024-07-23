using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Services.Installation;

public static class RetroRewindInstaller
{
    private static string RetroRewindVersionFilePath => Path.Combine(PathManager.RetroRewind6FolderPath, "version.txt");
    
    public static bool IsRetroRewindInstalled() => File.Exists(RetroRewindVersionFilePath);

    public static string CurrentRRVersion()
    {
        var versionFilePath = RetroRewindVersionFilePath;
        return IsRetroRewindInstalled() ? File.ReadAllText(versionFilePath).Trim() : "Not Installed";
    }

    public static async Task<bool> HandleNotInstalled()
    {
        var result = MessageBox.Show(
            "Your version of Retro Rewind could not be determined. Would you like to download Retro Rewind?",
            "Download Retro Rewind", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.No) return false;

        await InstallRetroRewind();
        return true;
    }

    public static async Task<bool> HandleOldVersion()
    {
        var result = MessageBox.Show(
            "Your version of Retro Rewind is too old to update. Would you like to reinstall Retro Rewind?",
            "Reinstall Retro Rewind", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.No) return false;

        await InstallRetroRewind();
        return true;
    }

    public static async Task InstallRetroRewind()
    {
        
        if (IsRetroRewindInstalled())
        {
            await HandleReinstall();
        }

        var tempZipPath = Path.Combine(PathManager.LoadFolderPath, "Temp", "RetroRewind.zip");
        await DownloadAndExtractRetroRewind(tempZipPath);
    }

    private static async Task HandleReinstall()
    {
        var result = MessageBox.Show("There are already files in your RetroRewind Folder. Would you like to install?",
            "Reinstall Retro Rewind", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.No) return;
        
        DeleteExistingRetroRewind();
    }

    private static async Task DownloadAndExtractRetroRewind(string tempZipPath)
    {
        var progressWindow = new ProgressWindow();
        progressWindow.ChangeExtraText("Downloading Retro Rewind...");
        progressWindow.Show();

        try
        {
            await DownloadHelper.DownloadToLocation(Endpoints.RRZipUrl, tempZipPath, progressWindow);
            progressWindow.ChangeExtraText("Extracting files...");
            var extractionPath = PathManager.RiivolutionWhWzFolderPath;
            ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);
        }
        finally
        {
            progressWindow.Close();
            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);
        }
    }
    

    private static void DeleteExistingRetroRewind()
    {
        var retroRewindPath = PathManager.RiivolutionWhWzFolderPath;
        if (Directory.Exists(retroRewindPath))
            Directory.Delete(retroRewindPath, true);
    }
}
