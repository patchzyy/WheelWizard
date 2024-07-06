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
        var retroRewindFolder = Path.Combine(loadPath, "Riivolution", "RetroRewind6");
        
        var retroRewindRRFolder = Path.Combine(loadPath, "Riivolution-RR", "RetroRewind6");

        return Directory.Exists(retroRewindFolder) || Directory.Exists(retroRewindRRFolder);
    }
    
    public static void InstallRRToSD()
    {
        MessageBox.Show("Installing Retro Rewind to SD/USB...");
        MessageBox.Show("Implement me!");
    }
    
    public static void InstallRRToUSB()
    {
        MessageBox.Show("Installing Retro Rewind to USB...");
        MessageBox.Show("Implement me!");
    }

    public static string CurrentRRVersion()
    {
        var loadPath = SettingsUtils.GetLoadPathLocation();
        var versionFilePath = Path.Combine(loadPath, "Riivolution", "RetroRewind6", "version.txt");
        if (File.Exists(versionFilePath)) return File.ReadAllText(versionFilePath);
        //try the RR folder
        versionFilePath = Path.Combine(loadPath, "Riivolution-RR", "RetroRewind6", "version.txt");
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
        MessageBox.Show("Retro Rewind is not installed. Starting installation.");
        await InstallRetroRewind();
        return true;
    }
    var allVersions = await GetAllVersionData();
    var updatesToApply = GetUpdatesToApply(currentVersion, allVersions);
    
    var progressWindow = new ProgressWindow();
    progressWindow.Show();
    
    int totalUpdates = updatesToApply.Count;
    int currentUpdateIndex = 1;
    foreach (var update in updatesToApply)
    {
        // MessageBox.Show($"Updating to version {update.Version}: {update.Description}");
        int progressPercentage = (currentUpdateIndex * 100) / totalUpdates;
        string status = $"Updating to version {update.Version}: {update.Description}";
        Application.Current.Dispatcher.Invoke(() => 
        {
            progressWindow.UpdateProgress(progressPercentage, status);
        });
        
        bool success = await DownloadAndApplyUpdate(update);
        if (!success)
        {
            progressWindow.Close();
            MessageBox.Show("Failed to apply an update. Aborting.");
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

private static async Task<bool> DownloadAndApplyUpdate((string Version, string Url, string Path, string Description) update)
{
    using var httpClient = new HttpClient();
    var tempZipPath = Path.GetTempFileName();

    try
    {
        var response = await httpClient.GetAsync(update.Url, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            MessageBox.Show($"Failed to download update: {update.Version}");
            return false;
        }

        using (var fs = new FileStream(tempZipPath, FileMode.Create))
        {
            await response.Content.CopyToAsync(fs);
        }

        // string extractionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dolphin Emulator", "Load", "Riivolution");
        var loadPath = SettingsUtils.GetLoadPathLocation();
        var extractionPath = Path.Combine(loadPath, "Riivolution");

        // Ensure the extraction path exists.
        Directory.CreateDirectory(extractionPath);

        // Extract the zip file.
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
        MessageBox.Show("Retro Rewind is already installed.");
        return;
    }
    
    var loadPath = SettingsUtils.GetLoadPathLocation();
    var tempZipPath = Path.Combine(loadPath, "Temp", "RetroRewind.zip");
    Directory.CreateDirectory(Path.GetDirectoryName(tempZipPath)); // Ensure the Temp directory exists
    
    try
    {
        using var httpClient = new HttpClient();
        
        var progressWindow = new ProgressWindow();
        progressWindow.Show();
        
        // Start the download
        var response = await httpClient.GetAsync(RetroRewindURL, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            MessageBox.Show("Failed to download Retro Rewind.");
            return;
        }
        
        long? totalDownloadSize = response.Content.Headers.ContentLength;
        var totalBytesDownloaded = 0L;
        
        using (var downloadStream = await response.Content.ReadAsStreamAsync())
        {
            using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
            {
                var buffer = new byte[81920]; // 80KB buffer
                int bytesRead;
                while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    totalBytesDownloaded += bytesRead;
                    
                    int progressPercentage = totalDownloadSize.HasValue
                        ? (int)((totalBytesDownloaded * 100) / totalDownloadSize.Value)
                        : 0;
                    
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        progressWindow.UpdateProgress(progressPercentage, "Downloading Retro Rewind... This may take a while.");
                    });
                }
            }
        }
        
        // Extract the zip to the Riivolution folder
        var extractionPath = Path.Combine(loadPath, "Riivolution");
        ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);
        
        // Clean up the downloaded zip file
        File.Delete(tempZipPath);
        
        Application.Current.Dispatcher.Invoke(() => 
        {
            progressWindow.Close();
        });
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred during the installation: {ex.Message}");
    }
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