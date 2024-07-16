using System.Threading.Tasks;
using CT_MKWII_WPF.Enums;

namespace CT_MKWII_WPF.Utils;

public class RRStatusManager
{

    public static async Task<ActionButtonStatus> GetCurrentStatus()
    {
        var serverEnabled = await RetroRewindInstaller.IsServerEnabled();
        if (!serverEnabled) return ActionButtonStatus.NoServer;
        var configCorrectAndExists = SettingsUtils.configCorrectAndExists();
        if (!configCorrectAndExists) return ActionButtonStatus.ConfigNotFinished;
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        if (!retroRewindInstalled) return ActionButtonStatus.noRR;
        bool retroRewindUpToDate;
        string latestRRVersion;
        if (!SettingsUtils.IsConfigFileFinishedSettingUp()) return ActionButtonStatus.ConfigNotFinished;
        retroRewindUpToDate = await RetroRewindInstaller.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        if (!retroRewindUpToDate) return ActionButtonStatus.OutOfDate;
        latestRRVersion = await RetroRewindInstaller.GetLatestVersionString();
        return ActionButtonStatus.UpToDate;
    }
}