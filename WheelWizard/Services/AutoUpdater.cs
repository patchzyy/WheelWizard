using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CT_MKWII_WPF.Utilities.Downloads;
using CT_MKWII_WPF.Views;

namespace CT_MKWII_WPF.Services;

public static class AutoUpdater
{
    public const string CurrentVersion = "1.1.2";
    private const string VersionFileUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/version.txt";
    private const string ReleasesUrl = "https://github.com/patchzyy/WheelWizard/releases/latest/download/WheelWizard.exe";
    
    public static async Task CheckForUpdatesAsync()
    {
        var response = await HttpClientHelper.GetAsync<string>(VersionFileUrl);
        if (!response.Succeeded || response.Content is null)
        {
            if (response.StatusCodeGroup == 4 || response.StatusCode is 503 or 504)
                MessageBox.Show("Unable to check if WheelWizard is up to date. " +
                                "\nYou might be experiencing network issues.");
            else MessageBox.Show("An error occurred while checking for updates. Please try again later. " +
                                 "\nError: " + response.StatusMessage);
            return;
        }
        if (response.Content.Trim() == CurrentVersion) return;
        var initiatedUpdate = MessageBox.Show("A new version of WheelWizard is available. Would you like to update?",
            "Update Available", MessageBoxButtons.YesNo);
        if (initiatedUpdate == DialogResult.Yes) await UpdateAsync();
    }

    private static async Task UpdateAsync()
    {
        string currentExecutablePath;
        using (var process = Process.GetCurrentProcess())
        {
            // Retrieve the path of the currently executing process
            currentExecutablePath = process.MainModule!.FileName;
        }
        
        var currentFolder = Path.GetDirectoryName(currentExecutablePath);
        if (currentFolder is null)
        {
            MessageBox.Show("Unable to update WheelWizard. " +
                            "Please ensure the application is located in a folder that can be written to.");
            return;
        }
        var newFilePath = Path.Combine(currentFolder, "CT-MKWII-WPF_new.exe");

        var progressWindow = new ProgressWindow();
        progressWindow.Show();

        await DownloadUtils.DownloadFileWithWindow(ReleasesUrl, newFilePath, progressWindow);

        // we need to wait a bit before running the batch file to ensure the file is saved on disk
        await Task.Delay(200);
        CreateAndRunBatchFile(currentExecutablePath, newFilePath);

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
}