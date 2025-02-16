using Semver;
using System.Text.Json;
using System.Threading.Tasks;
using WheelWizard.Helpers;
using WheelWizard.Models.Github;
using WheelWizard.Resources.Languages;
using WheelWizard.Views.Popups.Generic;

namespace WheelWizard.Services.Installation.AutoUpdater;

interface IAutoUpdateHandler   
{
    /// <summary>
    /// The current version of the application.
    /// </summary>
    string CurrentVersion { get; }

    /// <summary>
    /// Runs the entire update process flow (checks for updates, downloads, and installs them).
    /// </summary>
    Task CheckForUpdatesAsync();
}

public class AutoUpdaterHandler : IAutoUpdateHandler
{
    private readonly IUpdaterPlatform _updaterPlatform;
    //todo: look into assembly versioning c# best practices 
    public virtual string CurrentVersion => Installation.AutoUpdater.AutoUpdater.CurrentVersion;

    public AutoUpdaterHandler(IUpdaterPlatform updaterPlatform)
    {
        _updaterPlatform = updaterPlatform;
    }

    public async Task CheckForUpdatesAsync()
    {
        var latestRelease = await GetLatestReleaseAsync();
        if (latestRelease?.TagName is null)
            return;
        var asset = _updaterPlatform.GetAssetForCurrentPlatform(latestRelease);
        if (asset is null)
            return;

        var latestVersion = SemVersion.Parse(latestRelease.TagName.TrimStart('v'), SemVersionStyles.Any);
        var popupExtraText = Humanizer.ReplaceDynamic(Phrases.PopupText_NewVersionWhWz, latestVersion, CurrentVersion)!;
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var updateQuestion = await new YesNoWindow()
                .SetButtonText(Common.Action_Update, Common.Action_MaybeLater)
                .SetMainText(Phrases.PopupText_WhWzUpdateAvailable)
                .SetExtraText(popupExtraText)
                .AwaitAnswer();
            if (!updateQuestion)
                return;
            await _updaterPlatform.ExecuteUpdateAsync(asset.BrowserDownloadUrl);
        });
    }
    
    private async Task<GithubRelease?> GetLatestReleaseAsync()
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
            return null;
        }
        response.Content = response.Content.Trim('\0');
        var latestRelease = JsonSerializer.Deserialize<GithubRelease>(response.Content);
        if (latestRelease?.TagName is null) return null;
        
        var currentVersion = SemVersion.Parse(CurrentVersion, SemVersionStyles.Any);
        var latestVersion = SemVersion.Parse(latestRelease.TagName.TrimStart('v'), SemVersionStyles.Any);
        return currentVersion.ComparePrecedenceTo(latestVersion) < 0 ? latestRelease : null;
    }
}
