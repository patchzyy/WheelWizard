namespace CT_MKWII_WPF.Services;

public static class Endpoints
{
    // Retro Rewind
    public const string RRUrl = "http://update.zplwii.xyz:8000/";
    public const string RRZipUrl = RRUrl + "RetroRewind/zip/RetroRewind.zip";
    public const string RRVersionUrl = RRUrl + "RetroRewind/RetroRewindVersion.txt";
    public const string RRGroupsUrl = "http://zplwii.xyz/api/groups";

    // Wheel Wizard
    public const string WhWzStatusUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/status.txt";
    public const string WhWzLatestReleasesUrl = "https://api.github.com/repos/patchzyy/WheelWizard/releases/latest";
    public const string WhWzDiscordUrl = "https://discord.gg/vZ7T2wJnsq";
    public const string WhWzGithubUrl = "https://github.com/patchzyy/WheelWizard";
    public const string SupportLink = "https://ko-fi.com/wheelwizard";

    // Other
    public const string MiiStudioUrl = "https://qrcode.rc24.xyz/cgi-bin/studio.cgi";
    public const string MiiImageUrl = "https://studio.mii.nintendo.com/miis/image.png";
    public const string MarioCubeUrl = "https://repo.mariocube.com/WADs/Other/Mii%20Channel%20Symbols%20-%20HACS.wad";
}
