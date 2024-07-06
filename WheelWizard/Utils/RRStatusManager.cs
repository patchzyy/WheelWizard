using System.Threading.Tasks;

namespace CT_MKWII_WPF.Utils;

public class RRStatusManager
{
    public enum ActionButtonStatus
    {
        NoServer,
        NoDolphin,
        ConfigNotFinished,
        noRR,
        noRRActive,
        RRnotReady,
        OutOfDate,
        UpToDate
    }

    public static async Task<ActionButtonStatus> GetCurrentStatus()
    {
        var serverEnabled = await RetroRewindInstaller.IsServerEnabled();
        if (!serverEnabled) return ActionButtonStatus.NoServer;
        var configCorrectAndExists = SettingsUtils.configCorrectAndExists();
        if (!configCorrectAndExists) return ActionButtonStatus.ConfigNotFinished;
        var isUserFolderValid = DolphinInstaller.IsUserFolderValid();
        if (!isUserFolderValid) return ActionButtonStatus.NoDolphin;
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