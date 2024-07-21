using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Services.Configuration;
using CT_MKWII_WPF.Views;

namespace CT_MKWII_WPF.Services.Installation;

public static class RRUpdater
{
    public static async Task<bool> IsRRUpToDate(string currentVersion)
    {
        string latestVersion = await GetLatestVersionString();
        return currentVersion.Trim() == latestVersion.Trim();
    }
    
    private static async Task<string> GetLatestVersionString()
    {
        var response = await HttpClientHelper.GetAsync<string>(Endpoints.RRVersionUrl);
        if (!response.Succeeded || response.Content == null)
        {
            MessageBox.Show($"Failed to check for updates");
            return "Failed to check for updates";
        }
        return response.Content.Split('\n').Last().Split(' ')[0];
    }
    
    public static async Task<bool> UpdateRR()
    {
        var currentVersion = RetroRewindInstaller.CurrentRRVersion();

        if (await IsRRUpToDate(currentVersion))
        {
            MessageBox.Show("Retro Rewind is up to date.");
            return true;
        }

        if (currentVersion == "Not Installed")
        {
            return await RetroRewindInstaller.HandleNotInstalled();
        }

        //if currentversion is below 3.2.6 we need to do a full reinstall
        if (CompareVersions(currentVersion, "3.2.6") < 0)
        {
            return await RetroRewindInstaller.HandleOldVersion();
        }
        return await ApplyUpdates(currentVersion);
    }
    
    private static async Task<bool> ApplyUpdates(string currentVersion)
    {
        var allVersions = await GetAllVersionData();
        var updatesToApply = GetUpdatesToApply(currentVersion, allVersions);
        
        var progressWindow = new ProgressWindow();
        progressWindow.Show();

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
            progressWindow.Close();
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
     

        using var httpClient = new HttpClient();
        var allVersionsText = await httpClient.GetStringAsync(Endpoints.RRVersionUrl);
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
            window.ChangeExtraText($"Update {currentUpdateIndex}/{totalUpdates}: {update.Description}");
            await DownloadHelper.DownloadFileWithWindow(update.Url, tempZipPath, window);
            
            window.UpdateProgress(100, "Extracting update...","Extracting update...");
            var extractionPath = Path.Combine(loadPath, "Riivolution");
            Directory.CreateDirectory(extractionPath);
            ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);
        }
        finally
        {
            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);
        }

        return true;
    }
    
    
    
    
}