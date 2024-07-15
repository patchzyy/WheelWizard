using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF;
using CT_MKWII_WPF.Pages;
using CT_MKWII_WPF.Utils;

public static class RetroRewindInstaller
{
    public static bool IsRetroRewindInstalled()
    {
        var loadPath = SettingsUtils.GetLoadPathLocation();
        var versionFilePath = Path.Combine(loadPath, "Riivolution", "RetroRewind6", "version.txt");
        return File.Exists(versionFilePath);

    }

    public static string CurrentRRVersion()
    {
        var loadPath = SettingsUtils.GetLoadPathLocation();
        var versionFilePath = Path.Combine(loadPath, "Riivolution", "RetroRewind6", "version.txt");
        if (File.Exists(versionFilePath)) return File.ReadAllText(versionFilePath);
        return !File.Exists(versionFilePath) ? "Not Installed" : File.ReadAllText(versionFilePath);
    }

    public static async Task<bool> IsRRUpToDate(string currentVersion)
    {
        //make sure to ignore any spaces
        currentVersion = currentVersion.Trim();
        string latestVersion = await GetLatestVersionString();
        latestVersion = latestVersion.Trim();
        return currentVersion == latestVersion;
    }

    public static async Task<string> GetLatestVersionString()
    {
        string FullTextURLText = RRNetwork.Ip + "/RetroRewind/RetroRewindVersion.txt";
        try
        {
            using var httpClient = new HttpClient();
            string AllVersions = await httpClient.GetStringAsync(FullTextURLText);
            string[] lines = AllVersions.Split('\n');
            //now go to the last line split by spaces and grab index 0
            string latestVersion = lines[lines.Length - 1].Split(' ')[0];
            return latestVersion;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to check for updates: {ex.Message}");
            return "Failed to check for updates";
        }
        
    } 
    public static async Task<bool> UpdateRR()
    {
    string currentVersion = CurrentRRVersion();
    
    if (await IsRRUpToDate(currentVersion))
    {
        MessageBox.Show("Retro Rewind is up to date.");
        return true;
    }
    
    if (currentVersion == "Not Installed")
    {
        // ask user "Your version of RR could not be determent\n therefore we can not update, do you want to download RR?"
        var result = MessageBox.Show("Your version of Retro Rewind could not be determined. Therefore, we cannot update. Would you like to download Retro Rewind?", "Download Retro Rewind", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            await InstallRetroRewind();
        }
        return true;
    }
    
    //if currentversion is below 3.2.6 we need to do a full reinstall
    if (CompareVersions(currentVersion, "3.2.6") < 0)
    {
        var result = MessageBox.Show("Your version of Retro Rewind is too old to update. Would you like to reinstall Retro Rewind?", "Reinstall Retro Rewind", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.No) return false;
        await InstallRetroRewind();
        return true;
    }
    var allVersions = await GetAllVersionData();
    var updatesToApply = GetUpdatesToApply(currentVersion, allVersions);
    
    int totalUpdates = updatesToApply.Count;
    int currentUpdateIndex = 1;
    var progressWindow = new ProgressWindow();
    progressWindow.Show();
    foreach (var update in updatesToApply)
    {
        bool success = await DownloadAndApplyUpdate(update, totalUpdates, currentUpdateIndex, progressWindow);
        if (!success)
        {
            MessageBox.Show("Failed to apply an update. Aborting.");
            progressWindow.Close();
            return false;
        }
        currentUpdateIndex++;
        UpdateVersionFile(update.Version);
    }
    progressWindow.Close();
    return true;
}
    

private static void UpdateVersionFile(string newVersion)
{
    var loadPath = SettingsUtils.GetLoadPathLocation();
    var VersionFilePath = Path.Combine(loadPath, "Riivolution", "RetroRewind6", "version.txt");
    File.WriteAllText(VersionFilePath, newVersion);
}
private static async Task<List<(string Version, string Url, string Path, string Description)>> GetAllVersionData()
{
    List<(string Version, string Url, string Path, string Description)> versions = new List<(string Version, string Url, string Path, string Description)>();
    string versionUrl = RRNetwork.Ip + "/RetroRewind/RetroRewindVersion.txt";

    using var httpClient = new HttpClient();
    string allVersionsText = await httpClient.GetStringAsync(versionUrl);
    string[] lines = allVersionsText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (var line in lines)
    {
        string[] parts = line.Split(new[] { ' ' }, 4);
        if (parts.Length < 4) continue; // Skip if the line doesn't have enough parts.
        versions.Add((parts[0].Trim(), parts[1].Trim(), parts[2].Trim(), parts[3].Trim()));
    }

    return versions;
}

private static List<(string Version, string Url, string Path, string Description)> GetUpdatesToApply(string currentVersion, List<(string Version, string Url, string Path, string Description)> allVersions)
{
    var updatesToApply = new List<(string Version, string Url, string Path, string Description)>();
    allVersions.Sort((a,b) => CompareVersions(b.Version, a.Version)); // Sort in descending order
    foreach (var version in allVersions)
    {
        if (CompareVersions(version.Version, currentVersion) > 0)
        {
            updatesToApply.Add(version);
        }
        else
        {
            break; 
        }
    }
    updatesToApply.Reverse();
    return updatesToApply;
}

private static int CompareVersions(string v1, string v2)
{
    var parts1 = v1.Split('.').Select(int.Parse).ToArray();
    var parts2 = v2.Split('.').Select(int.Parse).ToArray();
    for (int i = 0; i < Math.Max(parts1.Length, parts2.Length); i++)
    {
        int p1 = i < parts1.Length ? parts1[i] : 0;
        int p2 = i < parts2.Length ? parts2[i] : 0;
        if (p1 != p2)
        {
            return p1.CompareTo(p2);
        }
    }
    return 0;
}

private static async Task<bool> DownloadAndApplyUpdate((string Version, string Url, string Path, string Description) update, int totalUpdates, int currentUpdateIndex, ProgressWindow window)
{
    var loadPath = SettingsUtils.GetLoadPathLocation();
    var tempZipPath = Path.GetTempFileName();
    try
    {
        string extratext = $"Update {currentUpdateIndex}/{totalUpdates}: {update.Description}";
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
        {
            File.Delete(tempZipPath);
        }
    }
    return true;
}




public static async Task InstallRetroRewind()
{
    string RetroRewindURL = RRNetwork.Ip + "/RetroRewind/zip/RetroRewind.zip";
    // Check if Retro Rewind is already installed
    if (IsRetroRewindInstalled())
    {
        var result = MessageBox.Show("There are already files in your RetroRewind Folder. Would you like to install?", "Reinstall Retro Rewind", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.No)
        {
            return;
        }
        var loadPath_ = SettingsUtils.GetLoadPathLocation();
        var rrWFC = Path.Combine(loadPath_, "Riivolution", "RetroRewind6", "save","RetroWFC");
        if (Directory.Exists(rrWFC))
        {
            var files = Directory.GetFiles(rrWFC, "rksys.dat", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                var regionFolder = Path.GetDirectoryName(files[0]);
                var regionFoldername = Path.GetFileName(regionFolder);
                var DATFILEDATA = await File.ReadAllBytesAsync(files[0]);
                var riiWFCregion = Path.Combine(loadPath_, "Riivolution", "riivolution", "save", "RetroWFC", regionFoldername);
                Directory.CreateDirectory(riiWFCregion);
                await File.WriteAllBytesAsync(Path.Combine(riiWFCregion, "rksys.dat"), DATFILEDATA);
            }
        }
        var retroRewindPath = Path.Combine(loadPath_, "Riivolution", "RetroRewind6");
        Directory.Delete(retroRewindPath, true);
    }
    
    var loadPath = SettingsUtils.GetLoadPathLocation();
    var tempZipPath = Path.Combine(loadPath, "Temp", "RetroRewind.zip");
    ProgressWindow progressWindow = new ProgressWindow();
    progressWindow.Show();
    await DownloadUtils.DownloadFileWithWindow(RetroRewindURL, tempZipPath, progressWindow, "Downloading Retro Rewind...");
    progressWindow.Close();
    // Extract the zip to the Riivolution folder
    var extractionPath = Path.Combine(loadPath, "Riivolution");
    ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);
    // Clean up the downloaded zip file
    File.Delete(tempZipPath);
    }




    public static async Task<bool> IsServerEnabled()
    {
        string serverUrl = RRNetwork.Ip;
        using (var httpClient = new HttpClient())
        {
            try
            {
                // Setting a reasonable timeout duration
                httpClient.Timeout = TimeSpan.FromSeconds(5);
            
                // Making a GET request to the server URL
                var response = await httpClient.GetAsync(serverUrl);
            
                // If the server responds with a success status code, it is reachable
                return response.IsSuccessStatusCode;
            }
            catch
            {
                // If an exception occurs (e.g., timeout), the server is not reachable
                return false;
            }
        }
    }
}