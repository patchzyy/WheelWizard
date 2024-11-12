using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models;
using CT_MKWII_WPF.Models.Github;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Views.Popups;
using Semver;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CT_MKWII_WPF.Services.Installation;

public static class AutoUpdater
{
    public const string CurrentVersion = "1.8.4";
    public static async Task CheckForUpdatesAsync()
    {
        var response = await HttpClientHelper.GetAsync<string>(Endpoints.WhWzLatestReleasesUrl);
        if (!response.Succeeded || response.Content is null)
        {
            HandleUpdateCheckError(response);
            return;
        }
        
        var latestRelease = JsonSerializer.Deserialize<GithubRelease>(response.Content);
        if (latestRelease?.TagName is null) return;
        
        var currentVersion = SemVersion.Parse(CurrentVersion, SemVersionStyles.Any);
        var latestVersion = SemVersion.Parse(latestRelease.TagName.TrimStart('v'), SemVersionStyles.Any);

        if (currentVersion.ComparePrecedenceTo(latestVersion) >= 0) return;
        if (IsAdministrator())
        {
            await UpdateAsync(latestRelease.Assets[0].BrowserDownloadUrl);
            return;
        }

        var updateQuestion = new YesNoWindow()
                             .SetButtonText(Common.Action_Update, Common.Action_MaybeLater)
                             .SetMainText(Phrases.PopupText_WhWzUpdateAvailable)
                             .SetExtraText(Humanizer.ReplaceDynamic(Phrases.PopupText_NewVersionWhWz,
                                                                    latestVersion, currentVersion));
        if (!updateQuestion.AwaitAnswer()) 
            return;
        
        var adminQuestion = new YesNoWindow()
                            .SetMainText(Phrases.PopupText_UpdateAdmin)  
                            .SetExtraText(Phrases.PopupText_UpdateAdminExplained);
        if (adminQuestion.AwaitAnswer())
            RestartAsAdmin();
        else
            await UpdateAsync(latestRelease.Assets[0].BrowserDownloadUrl);
    }
    
    private static void RestartAsAdmin()
    {
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            WorkingDirectory = Environment.CurrentDirectory, 
            FileName = GetActualExecutablePath(),
            Verb = "runas"
        };
        try
        {
            Process.Start(startInfo);
            Environment.Exit(0);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            new YesNoWindow().SetMainText(Phrases.PopupText_RestartAdminFail);
        }
    }
    
    private static string GetActualExecutablePath()
    {
        using (var process = Process.GetCurrentProcess())
        {
            return process.MainModule.FileName;
        }
    }
    
    private static bool IsAdministrator()
    {
        using (var identity = WindowsIdentity.GetCurrent())
        {
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    private static async Task UpdateAsync(string downloadUrl)
    {
        var currentExecutablePath = Process.GetCurrentProcess().MainModule!.FileName;
        var currentExecutableName = Path.GetFileNameWithoutExtension(currentExecutablePath);
        
        var currentFolder = Path.GetDirectoryName(currentExecutablePath);
        if (currentFolder is null)
        {
            MessageBoxWindow.ShowDialog(Phrases.PopupText_UnableUpdateWhWz_ReasonLocation);
            return;
        }
        var newFilePath = Path.Combine(currentFolder, currentExecutableName+"_new.exe");
        if (File.Exists(newFilePath))
            File.Delete(newFilePath);
        
        await DownloadHelper.DownloadToLocationAsync(downloadUrl, newFilePath, Phrases.PopupText_UpdateWhWz,
                                                Phrases.PopupText_LatestWhWzGithub, ForceGivenFilePath: true);
        //very important we set this to true


        // we need to wait a bit before running the batch file to ensure the file is saved on disk
        await Task.Delay(200);
        CreateAndRunPowerShellScript(currentExecutablePath, newFilePath);

        Environment.Exit(0);
    }

    private static void CreateAndRunPowerShellScript(string currentFilePath, string newFilePath)
    {
        var currentFolder = Path.GetDirectoryName(currentFilePath);
        if (currentFolder is null)
        {
            MessageBoxWindow.ShowDialog(Phrases.PopupText_UnableUpdateWhWz_ReasonLocation);
            return;
        }
        var scriptFilePath = Path.Combine(currentFolder, "update.ps1");
        var originalFileName = Path.GetFileName(currentFilePath);
        var newFileName = Path.GetFileName(newFilePath);
    
        var scriptContent = $@"
    Write-Output 'Starting update process...'
    
    # Wait for the original application to exit
    while (Get-Process -Name '{Path.GetFileNameWithoutExtension(originalFileName)}' -ErrorAction SilentlyContinue) {{
        Write-Output 'Waiting for {originalFileName} to exit...'
        Start-Sleep -Seconds 1
    }}
    
    Write-Output 'Deleting old executable...'
    $maxRetries = 5
    $retryCount = 0
    $deleted = $false
    
    while (-not $deleted -and $retryCount -lt $maxRetries) {{
        try {{
            Remove-Item -Path '{Path.Combine(currentFolder, originalFileName)}' -Force -ErrorAction Stop
            $deleted = $true
        }}
        catch {{
            Write-Output 'Failed to delete {originalFileName}. Retrying in 2 seconds...'
            Start-Sleep -Seconds 2
            $retryCount++
        }}
    }}
    
    if (-not $deleted) {{
        Write-Output 'Could not delete {originalFileName}. Update aborted.'
        pause
        exit 1
    }}
    
    Write-Output 'Renaming new executable...'
    try {{
        Rename-Item -Path '{Path.Combine(currentFolder, newFileName)}' -NewName '{originalFileName}' -ErrorAction Stop
    }}
    catch {{
        Write-Output 'Failed to rename {newFileName} to {originalFileName}. Update aborted.'
        pause
        exit 1
    }}
    
    Write-Output 'Starting the updated application...'
    Start-Process -FilePath '{Path.Combine(currentFolder, originalFileName)}'
    
    Write-Output 'Cleaning up...'
    Remove-Item -Path '{scriptFilePath}' -Force
    
    Write-Output 'Update completed successfully.'
    ";
    
        File.WriteAllText(scriptFilePath, scriptContent);
    
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptFilePath}\"",
            CreateNoWindow = false,
            UseShellExecute = false,
            WorkingDirectory = currentFolder
        };
    
        try
        {
            Process.Start(processStartInfo);
        }
        catch (Exception ex)
        {
            MessageBoxWindow.ShowDialog($"Failed to execute the update script. {ex.Message}");
        }
    }

    
    private static void HandleUpdateCheckError(HttpClientResult<string> response)
    {
        if (response.StatusCodeGroup == 4 || response.StatusCode is 503 or 504)
            MessageBoxWindow.ShowDialog(Phrases.PopupText_UnableUpdateWhWz_ReasonNetwork);
        else
        {
            MessageBoxWindow.ShowDialog("An error occurred while checking for updates. Please try again later. " +
                                    "\nError: " + response.StatusMessage);
        }
    }
}
