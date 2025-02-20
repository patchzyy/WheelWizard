using WheelWizard.Services.Settings;

namespace WheelWizard.Services;

public class LinuxHelper
{
    public static string DetectPackageManager()
    {
        if (LinuxVerifications.IsValidCommand("apt")) return "apt-get";
        if (LinuxVerifications.IsValidCommand("dnf")) return "dnf";
        if (LinuxVerifications.IsValidCommand("yum")) return "yum";
        if (LinuxVerifications.IsValidCommand("pacman")) return "pacman -S";
        return LinuxVerifications.IsValidCommand("zypper") ? "zypper" : string.Empty; // Unknown package manager
    }
}
