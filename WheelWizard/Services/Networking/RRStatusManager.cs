using System.Threading.Tasks;
using CT_MKWII_WPF.Models.Enums;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Validators;

namespace CT_MKWII_WPF.Services.Networking;

public static class RRStatusManager
{
    public static async Task<ActionButtonStatus> GetCurrentStatus()
    {
        var serverEnabled = await RetroRewindInstaller.IsServerEnabled();
        if (!serverEnabled) return ActionButtonStatus.NoServer;
        var configCorrectAndExists = ConfigValidator.ConfigCorrectAndExists();
        if (!configCorrectAndExists) return ActionButtonStatus.ConfigNotFinished;
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        if (!retroRewindInstalled) return ActionButtonStatus.NoRR;
        if (!ConfigValidator.IsConfigFileFinishedSettingUp()) return ActionButtonStatus.ConfigNotFinished;
        var retroRewindUpToDate = await RRUpdater.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        if (!retroRewindUpToDate) return ActionButtonStatus.OutOfDate;
        return ActionButtonStatus.UpToDate;
    }
}