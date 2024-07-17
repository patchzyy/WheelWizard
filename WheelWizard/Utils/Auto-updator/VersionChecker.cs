using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using CT_MKWII_WPF.Pages;

namespace CT_MKWII_WPF.Utils.Auto_updator;

public static class VersionChecker
{
    private const string VersionFileURL = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/version.txt";
    public const string CurrentVersion = "1.1.1";

    public static void CheckForUpdates()
    {
        using (var client = new WebClient())
        {
            try
            {
                var version = client.DownloadString(VersionFileURL).Trim();
                if (version != CurrentVersion)
                {
                    if (!IsAdministrator())
                    {
                        var result =
                            MessageBox.Show("A new version of WheelWizard is available. Would you like to update?",
                                "Update Available", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            RestartAsAdmin();
                        }
                    }
                    else
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

    private static bool IsAdministrator()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    private static void RestartAsAdmin()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = true;
        startInfo.WorkingDirectory = Environment.CurrentDirectory;
        startInfo.FileName = GetActualExecutablePath();
        startInfo.Verb = "runas";
        try
        {
            Process.Start(startInfo);
            Environment.Exit(0);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            MessageBox.Show("Administrator rights are required to perform the update. Please run the application as an administrator.");
        }
    }

    private static string GetActualExecutablePath()
    {
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

        await DownloadUtils.DownloadFileWithWindow(downloadUrl, newFilePath, progressWindow);

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

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = batchFilePath,
            CreateNoWindow = true,
            UseShellExecute = true,
            Verb = "runas"
        };

        try
        {
            Process.Start(psi);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            MessageBox.Show("Failed to run the update script with administrator rights. The update may not complete successfully.");
        }
    }
}