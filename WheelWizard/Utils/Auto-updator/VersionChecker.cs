using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using CT_MKWII_WPF.Pages;

namespace CT_MKWII_WPF.Utils.Auto_updator;

public static class VersionChecker
{
    private const string VersionFileURL = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/version.txt";
    private const string CurrentVersion = "1.0.5";
    
    public static string GetVersionNumber()
    {
        return CurrentVersion;
    }

    public static void CheckForUpdates()
    {
        using (var client = new WebClient())
        {
            try
            {
                var version = client.DownloadString(VersionFileURL).Trim();
                if (version != CurrentVersion)
                {
                    var result = MessageBox.Show("A new version of WheelWizard is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Update();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred while checking for updates. Please try again later. \n \nError: " + e.Message);
            }
        }
    }
    
    private static string GetActualExecutablePath()
    {
        // Use the process module's filename, which should be the actual .exe path
        using (var process = Process.GetCurrentProcess())
        {
            return process.MainModule.FileName;
        }
    }

    public static async Task Update()
    {
        string currentLocation = GetActualExecutablePath();
        var currentFolder = Path.GetDirectoryName(currentLocation);
        var downloadUrl = "https://github.com/patchzyy/WheelWizard/releases/latest/download/WheelWizard.exe";
        var newFilePath = Path.Combine(currentFolder, "CT-MKWII-WPF_new.exe");

        var progressWindow = new ProgressWindow();
        progressWindow.Show();

        // Download the new file
        await DownloadUpdateFile(downloadUrl, newFilePath, progressWindow);

        // Create and run the batch file
        CreateAndRunBatchFile(currentLocation, newFilePath);

        // Close the current program
        Environment.Exit(0);
    }

    private static void CreateAndRunBatchFile(string currentFilePath, string newFilePath)
    {
        var batchFilePath = Path.Combine(Path.GetDirectoryName(currentFilePath), "update.bat");
        var originalFileName = Path.GetFileName(currentFilePath);
        var newFileName = Path.GetFileName(newFilePath);

        var batchContent = @"
@echo off
timeout /t 2 /nobreak
del """ + originalFileName + @"""
rename """ + newFileName + @""" """ + originalFileName + @"""
start """" """ + originalFileName + @"""
del ""%~f0""
";

        File.WriteAllText(batchFilePath, batchContent);

        Process.Start(new ProcessStartInfo(batchFilePath) { CreateNoWindow = true });
    }

    static async Task DownloadUpdateFile(string url, string filePath, ProgressWindow progressWindow)
    {
        using (HttpClient client = new HttpClient())
        {
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true))
                {
                    var downloadedBytes = 0;
                    var bufferSize = 8192;
                    var buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        var progress = totalBytes == -1 ? 0 : (int)((float)downloadedBytes / totalBytes * 100);
                        var status = $"Downloading... {downloadedBytes}/{totalBytes} bytes";

                        progressWindow.Dispatcher.Invoke(() =>
                        {
                            progressWindow.UpdateProgress(progress, status);
                        });
                    }
                }
            }
        }
    }
}