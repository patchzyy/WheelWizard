using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Services.Networking;
using CT_MKWII_WPF.Utilities.Configuration;
using CT_MKWII_WPF.Utilities.Downloads;
using CT_MKWII_WPF.Views;

namespace CT_MKWII_WPF.Services.Installation;

public static class RetroRewindInstaller
{
    private const string VersionFileName = "version.txt";
    private const string RetroRewindFolderName = "RetroRewind6";
    private const string RiivolutionFolderName = "Riivolution";
    private static readonly HttpClient HttpClient = new();
    
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

    public static async Task<bool> IsRRUpToDate(string currentVersion)
    {
        string latestVersion = await GetLatestVersionString();
        return currentVersion.Trim() == latestVersion.Trim();
    }

    public static async Task<string> GetLatestVersionString()
    {
        var fullTextURLText = $"{RRNetwork.Ip}/RetroRewind/RetroRewindVersion.txt";
        try
        {
            var allVersions = await HttpClient.GetStringAsync(fullTextURLText);
            //now go to the last line split by spaces and grab index 0
            return allVersions.Split('\n').Last().Split(' ')[0];
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to check for updates: {ex.Message}");
            return "Failed to check for updates";
        }
    }

    public static async Task<bool> UpdateRR()
    {
        var currentVersion = CurrentRRVersion();

        if (await IsRRUpToDate(currentVersion))
        {
            MessageBox.Show("Retro Rewind is up to date.");
            return true;
        }

        if (currentVersion == "Not Installed")
        {
            return await HandleNotInstalled();
        }

        //if currentversion is below 3.2.6 we need to do a full reinstall
        if (CompareVersions(currentVersion, "3.2.6") < 0)
        {
            return await HandleOldVersion();
        }
        return await ApplyUpdates(currentVersion);
    }
    
    private static async Task<bool> HandleNotInstalled()
    {
        var result = MessageBox.Show(
            "Your version of Retro Rewind could not be determined. Would you like to download Retro Rewind?",
            "Download Retro Rewind", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.No) return false;
        
        await InstallRetroRewind();
        return true;
    }
    
    private static async Task<bool> HandleOldVersion()
    {
        var result = MessageBox.Show(
            "Your version of Retro Rewind is too old to update. Would you like to reinstall Retro Rewind?",
            "Reinstall Retro Rewind", MessageBoxButton.YesNo);
            
        if (result == MessageBoxResult.No) return false;
            
        await InstallRetroRewind();
        return true;
    }

    private static async Task<bool> ApplyUpdates(string currentVersion)
    {
        var allVersions = await GetAllVersionData();
        var updatesToApply = GetUpdatesToApply(currentVersion, allVersions);
        
        var progressWindow = new ProgressWindow();
        progressWindow.Show();
        try
        {
            for (int i = 0; i < updatesToApply.Count; i++)
            {
                var update = updatesToApply[i];
                var success = await DownloadAndApplyUpdate(update, updatesToApply.Count, i + 1, progressWindow);
                if (!success)
                {
                    MessageBox.Show("Failed to apply an update. Aborting.");
                    progressWindow.Close();
                    return false;
                }

                UpdateVersionFile(update.Version);
            }
        }
        finally
        {
            progressWindow.Close();
        }

        return true;
    }


    private static void UpdateVersionFile(string newVersion)
    {
        var loadPath = PathManager.GetLoadPathLocation();
        var versionFilePath = Path.Combine(loadPath, "Riivolution", "RetroRewind6", "version.txt");
        File.WriteAllText(versionFilePath, newVersion);
    }

    private static async Task<List<(string Version, string Url, string Path, string Description)>> GetAllVersionData()
    {
        var versions = new List<(string Version, string Url, string Path, string Description)>();
        var versionUrl = RRNetwork.Ip + "/RetroRewind/RetroRewindVersion.txt";

        using var httpClient = new HttpClient();
        var allVersionsText = await httpClient.GetStringAsync(versionUrl);
        var lines = allVersionsText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parts = line.Split(new[] { ' ' }, 4);
            if (parts.Length < 4) continue;
            versions.Add((parts[0].Trim(), parts[1].Trim(), parts[2].Trim(), parts[3].Trim()));
        }

        return versions;
    }

    private static List<(string Version, string Url, string Path, string Description)> GetUpdatesToApply(
        string currentVersion, List<(string Version, string Url, string Path, string Description)> allVersions)
    {
        var updatesToApply = new List<(string Version, string Url, string Path, string Description)>();
        allVersions.Sort((a, b) => CompareVersions(b.Version, a.Version)); // Sort in descending order
        foreach (var version in allVersions)
        {
            if (CompareVersions(version.Version, currentVersion) > 0)
                updatesToApply.Add(version);
            else break;
        }
        
        updatesToApply.Reverse();
        return updatesToApply;
    }

    private static int CompareVersions(string v1, string v2)
    {
        var parts1 = v1.Split('.').Select(int.Parse).ToArray();
        var parts2 = v2.Split('.').Select(int.Parse).ToArray();
        for (var i = 0; i < Math.Max(parts1.Length, parts2.Length); i++)
        {
            var p1 = i < parts1.Length ? parts1[i] : 0;
            var p2 = i < parts2.Length ? parts2[i] : 0;
            if (p1 != p2) return p1.CompareTo(p2);
        }

        return 0;
    }

    private static async Task<bool> DownloadAndApplyUpdate(
        (string Version, string Url, string Path, string Description) update, int totalUpdates, int currentUpdateIndex,
        ProgressWindow window)
    {
        var loadPath = PathManager.GetLoadPathLocation();
        var tempZipPath = Path.GetTempFileName();
        try
        {
            var extratext = $"Update {currentUpdateIndex}/{totalUpdates}: {update.Description}";
            await DownloadUtils.DownloadFileWithWindow(update.Url, tempZipPath, window, extratext);
            
            window.UpdateProgress(100, "Extracting update...");
            var extractionPath = Path.Combine(loadPath, "Riivolution");
            Directory.CreateDirectory(extractionPath);
            ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while applying the update: {ex.Message}");
            return false;
        }
        finally
        {
            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);
        }

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
            await DownloadUtils.DownloadFileWithWindow(retroRewindURL, tempZipPath, progressWindow, "Downloading Retro Rewind...");
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