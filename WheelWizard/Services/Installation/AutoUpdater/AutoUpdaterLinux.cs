using System.Linq;
using WheelWizard.Helpers;
using WheelWizard.Models.Github;
using WheelWizard.Resources.Languages;
using WheelWizard.Views.Popups.Generic;
using Semver;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WheelWizard.Services.Installation;
public static class AutoUpdaterLinux
{
    public const string CurrentVersion = AutoUpdater.CurrentVersion;
    public static async Task CheckForUpdatesAsync()
    {
        var response = await HttpClientHelper.GetAsync<string>(Endpoints.WhWzLatestReleasesUrl);
        if (!response.Succeeded || response.Content is null)
        {
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
        if (latestRelease?.TagName is null)
            return;

        var currentVersion = SemVersion.Parse(CurrentVersion, SemVersionStyles.Any);
        var latestVersion = SemVersion.Parse(latestRelease.TagName.TrimStart('v'), SemVersionStyles.Any);

        // If the current version is equal to or newer than the latest release, nothing to do.
        if (currentVersion.ComparePrecedenceTo(latestVersion) >= 0)
            return;

        // Locate the Linux asset (assuming its URL contains "WheelWizard_Linux")
        var linuxAsset = latestRelease.Assets.FirstOrDefault(asset =>
            asset.BrowserDownloadUrl.Contains("WheelWizard_Linux", StringComparison.OrdinalIgnoreCase));
        if (linuxAsset == null)
        {
            // No Linux asset available; nothing to update.
            return;
        }

        // Ask the user if they want to update
        var popupExtraText = Humanizer.ReplaceDynamic(Phrases.PopupText_NewVersionWhWz, latestVersion, currentVersion);
        var updateQuestion = await new YesNoWindow()
                                .SetButtonText(Common.Action_Update, Common.Action_MaybeLater)
                                .SetMainText(Phrases.PopupText_WhWzUpdateAvailable)
                                .SetExtraText(popupExtraText)
                                .AwaitAnswer();
        if (!updateQuestion)
            return;

        await UpdateAsync(linuxAsset.BrowserDownloadUrl);
    }

    private static async Task UpdateAsync(string downloadUrl)
    {
        // Get the current executable details.
        var currentExecutablePath = Process.GetCurrentProcess().MainModule!.FileName;
        var currentExecutableName = Path.GetFileName(currentExecutablePath);
        var currentFolder = Path.GetDirectoryName(currentExecutablePath);
        if (currentFolder is null)
        {
            await new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Warning)
                .SetTitleText("Unable to update Wheel Wizard")
                .SetInfoText(Phrases.PopupText_UnableUpdateWhWz_ReasonLocation)
                .ShowDialog();
            return;
        }

        // Download the new file to a temporary location (e.g. "WheelWizard_Linux_new")
        var newFilePath = Path.Combine(currentFolder, currentExecutableName + "_new");
        if (File.Exists(newFilePath))
            File.Delete(newFilePath);

        await DownloadHelper.DownloadToLocationAsync(
            downloadUrl, 
            newFilePath, 
            Phrases.PopupText_UpdateWhWz,
            Phrases.PopupText_LatestWhWzGithub,
            ForceGivenFilePath: true);

        // Wait briefly to ensure the file is fully written
        await Task.Delay(200);

        // Create and run the Bash script that will perform the update.
        CreateAndRunShellScript(currentExecutablePath, newFilePath);

        // Exit the current process to allow the update to proceed.
        Environment.Exit(0);
    }

    private static void CreateAndRunShellScript(string currentFilePath, string newFilePath)
    {
        var currentFolder = Path.GetDirectoryName(currentFilePath);
        if (currentFolder is null)
        {
            new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Warning)
                .SetTitleText("Unable to update Wheel Wizard")
                .SetInfoText(Phrases.PopupText_UnableUpdateWhWz_ReasonLocation)
                .ShowDialog();
            return;
        }

        var scriptFilePath = Path.Combine(currentFolder, "update.sh");
        var originalFileName = Path.GetFileName(currentFilePath);
        var newFileName = Path.GetFileName(newFilePath);

        // The Bash script waits until the current process is no longer running, replaces the old binary, sets execute permissions, starts the new binary, and cleans up.
        var scriptContent = $@"#!/bin/bash
echo 'Starting update process...'

# Give a short delay to ensure the application has exited
sleep 1

echo 'Replacing old executable...'
rm -f ""{Path.Combine(currentFolder, originalFileName)}""
mv ""{Path.Combine(currentFolder, newFileName)}"" ""{Path.Combine(currentFolder, originalFileName)}""
chmod +x ""{Path.Combine(currentFolder, originalFileName)}""

echo 'Starting the updated application...'
nohup ""{Path.Combine(currentFolder, originalFileName)}"" > /dev/null 2>&1 &

echo 'Cleaning up...'
rm -- ""{scriptFilePath}""

echo 'Update completed successfully.'
";
        File.WriteAllText(scriptFilePath, scriptContent);

        // Ensure the script is executable.
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = $"+x \"{scriptFilePath}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            })?.WaitForExit();
        }
        catch (Exception ex)
        {
            new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Error)
                .SetTitleText("Unable to update Wheel Wizard")
                .SetInfoText($"Failed to set execute permission for the update script. {ex.Message}")
                .ShowDialog();
        }

        // Start the Bash script.
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "bash",
            Arguments = $"\"{scriptFilePath}\"",
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
                .SetTitleText("Unable to update Wheel Wizard")
                .SetInfoText($"Failed to execute the update script. {ex.Message}")
                .ShowDialog();
        }
    }
}

