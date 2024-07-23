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
    private const string VersionFileName = "version.txt";
    private static string GetVersionFilePath() => Path.Combine(PathManager.RetroRewind6FolderPath, VersionFileName);
    
    private static string LoadFolderPath => PathManager.LoadFolderPath;
    
    public static bool IsRetroRewindInstalled()
    {
        var versionFilePath = GetVersionFilePath();
        return File.Exists(versionFilePath);
    }

    public static string CurrentRRVersion()
    {
        var versionFilePath = GetVersionFilePath();
        return File.Exists(versionFilePath) ? File.ReadAllText(versionFilePath).Trim() : "Not Installed";
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

        var tempZipPath = Path.Combine(LoadFolderPath, "Temp", "RetroRewind.zip");
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
            var extractionPath = Path.Combine(PathManager.RiivolutionWhWzPath);
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
        var retroRewindPath = Path.Combine(PathManager.RiivolutionWhWzPath);
        if (Directory.Exists(retroRewindPath))
            Directory.Delete(retroRewindPath, true);
    }
}
