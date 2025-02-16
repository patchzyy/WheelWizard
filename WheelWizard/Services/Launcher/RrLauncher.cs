using System.Threading.Tasks;
using WheelWizard.Models.Enums;
using WheelWizard.Services.Settings;

namespace WheelWizard.Services.Launcher;

public class RrLauncher : RrBaseLauncher
{
    public override string GameTitle => "Retro Rewind";
    public override Task Launch()
    {
        SettingsManager.RETRO_REWIND_GAMEMODE.Set(RrGameMode.RETRO_TRACKS);
        return DoLaunch();
    }
}
