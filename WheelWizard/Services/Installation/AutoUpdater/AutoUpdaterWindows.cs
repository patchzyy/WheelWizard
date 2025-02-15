using Semver;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using WheelWizard.Helpers;
using WheelWizard.Models.Github;
using WheelWizard.Resources.Languages;
using WheelWizard.Views.Popups.Generic;

namespace WheelWizard.Services.Installation;
public static class AutoUpdaterWindows
{
    public const string CurrentVersion = "2.0.0";
    public static async Task CheckForUpdatesAsync()
    {
        var response = await HttpClientHelper.GetAsync<string>(Endpoints.WhWzLatestReleasesUrl);
        if (!response.Succeeded || response.Content is null)
        {
            // If it failed, it can be due to many reasons. We don't want to always throw an error,
            // since most of the times its simply because the wifi is not on or something
            // It's not useful to send that error in that case so we filter those out first.
            if (response.StatusCodeGroup != 4 && response.StatusCode is not 503 and not 504)
            {
                await new MessageBoxWindow()
                    .SetMessageType(MessageBoxWindow.MessageType.Error)
                    .SetTitleText("Failed to check for updates")
                    .SetInfoText("An error occurred while checking for updates. Please try again later. " + 
                                  "\nError: " + response.StatusMessage)
                    .ShowDialog();
            }
            return;
        }
        response.Content = response.Content.Trim('\0');
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

        var popupExtraText = Humanizer.ReplaceDynamic(Phrases.PopupText_NewVersionWhWz, latestVersion, currentVersion);
        var updateQuestion = await new YesNoWindow()
                             .SetButtonText(Common.Action_Update, Common.Action_MaybeLater)
                             .SetMainText(Phrases.PopupText_WhWzUpdateAvailable)
                             .SetExtraText(popupExtraText)
                             .AwaitAnswer();
        if (!updateQuestion) 
            return;
        
        var adminQuestion = new YesNoWindow()
                            .SetMainText(Phrases.PopupText_UpdateAdmin)  
                            .SetExtraText(Phrases.PopupText_UpdateAdminExplained);
        
        if (await adminQuestion.AwaitAnswer()) RestartAsAdmin();
        else await UpdateAsync(latestRelease.Assets[0].BrowserDownloadUrl);
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
        catch (Exception e)
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
            await new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Warning)
                .SetTitleText("Unable to update wheel wizard")
                .SetInfoText(Phrases.PopupText_UnableUpdateWhWz_ReasonLocation).ShowDialog();
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
            new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Warning)
                .SetTitleText("Unable to update wheel wizard")
                .SetTitleText(Phrases.PopupText_UnableUpdateWhWz_ReasonLocation)
                .ShowDialog();
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
            new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Error)
                .SetTitleText("Unable to update wheel wizard")
                .SetTitleText($"Failed to execute the update script. {ex.Message}")
                .ShowDialog();
        }
    }
}
