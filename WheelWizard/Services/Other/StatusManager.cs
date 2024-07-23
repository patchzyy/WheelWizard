using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Enums;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Settings;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Services.Other;

// This class will go over each individual dependency and check if it is all correct.
// If it is not, it will return the appropriate status. where the rest of the application can do whatever it wants with it.
public static class StatusManager
{
    public static async Task<WheelWizardStatus> GetCurrentStatus()
    {
        var configCorrectAndExists = ConfigValidator.ConfigCorrectAndExists();
        if (!configCorrectAndExists) return WheelWizardStatus.ConfigNotFinished;
        var retroRewindInstalled = RetroRewindInstaller.IsRetroRewindInstalled();
        if (!retroRewindInstalled) return WheelWizardStatus.NoRR;
        if (!ConfigValidator.IsConfigFileFinishedSettingUp()) return WheelWizardStatus.ConfigNotFinished;
        var serverEnabled = await HttpClientHelper.GetAsync<string>(Endpoints.RRUrl);
        if (!serverEnabled.Succeeded) return WheelWizardStatus.NoServer;
        var retroRewindUpToDate = await RetroRewindUpdater.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        if (!retroRewindUpToDate) return WheelWizardStatus.OutOfDate;
        return WheelWizardStatus.UpToDate;
    }
}
