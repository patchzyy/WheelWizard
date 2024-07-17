using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using CT_MKWII_WPF.Utilities.Downloads;
using CT_MKWII_WPF.Views;

namespace CT_MKWII_WPF.Utilities.Auto_updator;

public static class VersionChecker
{
    private const string VersionFileUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/version.txt";
    private const string DownloadUrl = "https://github.com/patchzyy/WheelWizard/releases/latest/download/WheelWizard.exe";
    
    public const string CurrentVersion = "1.1.3";


    public static void CheckForUpdates()
    {
        using (var client = new WebClient())
        {
            try
            {
                var version = client.DownloadString(VersionFileUrl).Trim();
                if (version == CurrentVersion) return;
                var result = MessageBox.Show("A new version of WheelWizard is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes) Update();
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
            return process.MainModule!.FileName;
        }
    }

    public static async Task Update()
    {
        var currentLocation = GetActualExecutablePath();
        var currentFolder = Path.GetDirectoryName(currentLocation);
        var newFilePath = Path.Combine(currentFolder, "CT-MKWII-WPF_new.exe");

        var progressWindow = new ProgressWindow();
        progressWindow.Show();
        
        await DownloadUtils.DownloadFileWithWindow(DownloadUrl, newFilePath, progressWindow);

        // we need to wait a bit before running the batch file to ensure the file is saved on disk
        await Task.Delay(200);
        CreateAndRunBatchFile(currentLocation, newFilePath);

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
