using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Services.Configuration;
using CT_MKWII_WPF.Services.Networking;
using CT_MKWII_WPF.Views;

namespace CT_MKWII_WPF.Services.Installation;

public static class RetroRewindInstaller
{
    private const string VersionFileName = "version.txt";
    private const string RetroRewindFolderName = "RetroRewind6";
    private const string RiivolutionFolderName = "Riivolution";
    
    private static string GetVersionFilePath() => Path.Combine(PathManager.GetLoadPathLocation(), RiivolutionFolderName, RetroRewindFolderName, VersionFileName);
    public static bool IsRetroRewindInstalled()
    {
        string versionFilePath = GetVersionFilePath();
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
        var retroRewindURL = $"{RRNetwork.Ip}/RetroRewind/zip/RetroRewind.zip";
        var loadPath = PathManager.GetLoadPathLocation();

        if (IsRetroRewindInstalled())
        {
            await HandleReinstall(loadPath);
        }

        var tempZipPath = Path.Combine(loadPath, "Temp", "RetroRewind.zip");
        await DownloadAndExtractRetroRewind(retroRewindURL, tempZipPath, loadPath);
    }

    private static async Task HandleReinstall(string loadPath)
    {
        var result = MessageBox.Show("There are already files in your RetroRewind Folder. Would you like to install?",
            "Reinstall Retro Rewind", MessageBoxButton.YesNo);
            
        if (result == MessageBoxResult.No) return;

        await BackupRksysFile(loadPath);
        DeleteExistingRetroRewind(loadPath);
    }
    
    private static async Task DownloadAndExtractRetroRewind(string retroRewindURL, string tempZipPath, string loadPath)
    {
        var progressWindow = new ProgressWindow();
        progressWindow.Show();
            
        try
        {
            await DownloadHelper.DownloadFileWithWindow(retroRewindURL, tempZipPath, progressWindow, "Downloading Retro Rewind...");
            var extractionPath = Path.Combine(loadPath, RiivolutionFolderName);
            ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);
        }
        finally
        {
            progressWindow.Close();
            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);
        }
    }
    
    private static async Task BackupRksysFile(string loadPath)
    {
        var rrWfc = Path.Combine(loadPath, RiivolutionFolderName, RetroRewindFolderName, "save", "RetroWFC");
        if (!Directory.Exists(rrWfc)) return;

        var rksysFiles = Directory.GetFiles(rrWfc, "rksys.dat", SearchOption.AllDirectories);
        if (rksysFiles.Length == 0) return;

        var sourceFile = rksysFiles[0];
        var regionFolder = Path.GetDirectoryName(sourceFile);
        var regionFolderName = Path.GetFileName(regionFolder);
        var datFileData = await File.ReadAllBytesAsync(sourceFile);
        if (regionFolderName == null) return;
        var destinationFolder = Path.Combine(loadPath, RiivolutionFolderName, "riivolution", "save", "RetroWFC", regionFolderName);
        Directory.CreateDirectory(destinationFolder);
        await File.WriteAllBytesAsync(Path.Combine(destinationFolder, "rksys.dat"), datFileData);
    }
    
    private static void DeleteExistingRetroRewind(string loadPath)
    {
        var retroRewindPath = Path.Combine(loadPath, RiivolutionFolderName, RetroRewindFolderName);
        if (Directory.Exists(retroRewindPath))
            Directory.Delete(retroRewindPath, true);
    }

    public static async Task<bool> IsServerEnabled()
    {
        var serverUrl = RRNetwork.Ip;
        using (var httpClient = new HttpClient())
        {
            try
            {
                httpClient.Timeout = TimeSpan.FromSeconds(5);
                var response = await httpClient.GetAsync(serverUrl);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}