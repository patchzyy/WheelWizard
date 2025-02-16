using System.Threading.Tasks;
using WheelWizard.Models.Enums;
using WheelWizard.Services.Settings;

namespace WheelWizard.Services.Launcher;

public class RrCtsLauncher : RrBaseLauncher
{
    public override string GameTitle => "Custom Tracks";
    public override Task Launch()
    {
        SettingsManager.RETRO_REWIND_GAMEMODE.Set(RrGameMode.CUSTOM_TRACKS);
        return DoLaunch();
    }
}
