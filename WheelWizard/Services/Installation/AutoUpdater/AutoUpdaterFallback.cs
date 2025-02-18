using Semver;
using System.Threading.Tasks;
using WheelWizard.Models.Github;
using WheelWizard.Views.Popups.Generic;


namespace WheelWizard.Services.Installation.AutoUpdater;

public class AutoUpdaterFallback : IUpdaterPlatform
{
    public GithubAsset? GetAssetForCurrentPlatform(GithubRelease release)
    {
        var latestVersion = SemVersion.Parse(release.TagName.TrimStart('v'), SemVersionStyles.Any);
        var currentVersion = SemVersion.Parse(AutoUpdater.CurrentVersion, SemVersionStyles.Any);
        if (currentVersion.ComparePrecedenceTo(latestVersion) >= 0) return null;

        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            new MessageBoxWindow()
                .SetTitleText("New Wheel Wizard version")
                .SetInfoText("There is a new Wheel Wizard version available!\n" +
                             $"Version {release.TagName.TrimStart('v')} (You are currently on {AutoUpdater.CurrentVersion})\n" +
                             "You can manually update it by going to the github releases at: " +
                             "https://github.com/patchzyy/WheelWizard/releases")
                .Show();
        });
        
        return null;
    }

    public Task ExecuteUpdateAsync(string downloadUrl) => Task.CompletedTask;
}
