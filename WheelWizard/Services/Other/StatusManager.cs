using System.Threading.Tasks;
using WheelWizard.Helpers;
using WheelWizard.Models.Enums;
using WheelWizard.Services.Installation;
using WheelWizard.Services.Settings;

namespace WheelWizard.Services.Other;

// This class will go over each individual dependency and check if it is all correct.
// If it is not, it will return the appropriate status. where the rest of the application can do whatever it wants with it.
public static class StatusManager
{
    public static async Task<WheelWizardStatus> GetCurrentStatus()
    {
        if (!SettingsHelper.PathsSetupCorrectly()) 
            return WheelWizardStatus.ConfigNotFinished;
        
        var serverEnabled = await HttpClientHelper.GetAsync<string>(Endpoints.RRUrl);
        if (!RetroRewindInstaller.IsRetroRewindInstalled())
        {
            if (RetroRewindInstaller.HasOldRR()) return WheelWizardStatus.RRNotReady;
            return !serverEnabled.Succeeded ? WheelWizardStatus.NoServer : WheelWizardStatus.NoRR;
        }
        if (!serverEnabled.Succeeded) 
            return WheelWizardStatus.NoServerButInstalled;
        
        var retroRewindUpToDate = await RetroRewindUpdater.IsRRUpToDate(RetroRewindInstaller.CurrentRRVersion());
        if (!retroRewindUpToDate) 
            return WheelWizardStatus.OutOfDate;
        
        return WheelWizardStatus.UpToDate;
    }
}
