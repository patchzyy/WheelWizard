using WheelWizard.Services.Settings;

namespace WheelWizard.Services;

public class LinuxHelper
{
    public static string DetectPackageManager()
    {
        if (LinuxDolphinInstaller.IsValidCommand("apt")) return "apt-get";
        if (LinuxDolphinInstaller.IsValidCommand("dnf")) return "dnf";
        if (LinuxDolphinInstaller.IsValidCommand("yum")) return "yum";
        if (LinuxDolphinInstaller.IsValidCommand("pacman")) return "pacman -S";
        return LinuxDolphinInstaller.IsValidCommand("zypper") ? "zypper" : string.Empty; // Unknown package manager
    }
}
