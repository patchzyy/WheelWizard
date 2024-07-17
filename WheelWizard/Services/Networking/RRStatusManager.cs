using System.Threading.Tasks;
using CT_MKWII_WPF.Models.Enums;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Utilities.Configuration;

namespace CT_MKWII_WPF.Services.Networking;

public class RRStatusManager
{
    public static async Task<ActionButtonStatus> GetCurrentStatus()
    {
        var serverEnabled = await RetroRewindInstaller.IsServerEnabled();
        if (!serverEnabled) return ActionButtonStatus.NoServer;
        var configCorrectAndExists = ConfigValidator.ConfigCorrectAndExists();
        if (!configCorrectAndExists) return ActionButtonStatus.ConfigNotFinished;
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        if (!retroRewindInstalled) return ActionButtonStatus.NoRR;
        bool retroRewindUpToDate;
        string latestRRVersion;
        if (!ConfigValidator.IsConfigFileFinishedSettingUp()) return ActionButtonStatus.ConfigNotFinished;
        retroRewindUpToDate = await RetroRewindInstaller.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        if (!retroRewindUpToDate) return ActionButtonStatus.OutOfDate;
        latestRRVersion = await RetroRewindInstaller.GetLatestVersionString();
        return ActionButtonStatus.UpToDate;
    }
}