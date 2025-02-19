namespace WheelWizard.Services;

public static class Endpoints
{
    // Retro Rewind
    public const string RRUrl = "http://update.zplwii.xyz:8000/";
    public const string RRZipUrl = RRUrl + "RetroRewind/zip/RetroRewind.zip";
    public const string RRVersionUrl = RRUrl + "RetroRewind/RetroRewindVersion.txt";
    public const string RRVersionDeleteUrl = RRUrl + "RetroRewind/RetroRewindDelete.txt";
    public const string RRGroupsUrl = "http://zplwii.xyz/api/groups";
    public const string RRDiscordUrl = "https://discord.gg/yH3ReN8EhQ";

    // Wheel Wizard
    public const string WhWzStatusUrl = "https://raw.githubusercontent.com/TeamWheelWizard/WheelWizard-Data/main/status.json";
    public const string WhWzLatestReleasesUrl = "https://api.github.com/repos/TeamWheelWizard/WheelWizard/releases/latest";
    public const string WhWzDiscordUrl = "https://discord.gg/vZ7T2wJnsq";
    public const string WhWzGithubUrl = "https://github.com/TeamWheelWizard/WheelWizard";
    public const string SupportLink = "https://ko-fi.com/wheelwizard";

    // Other
    public const string MiiStudioUrl = "https://qrcode.rc24.xyz/cgi-bin/studio.cgi";
    public const string MiiImageUrl = "https://studio.mii.nintendo.com/miis/image.png";
    public const string MiiChannelWAD = "-";
    
    //GameBanana
    public const string GameBananaBaseUrl = "https://gamebanana.com/apiv11";
}
