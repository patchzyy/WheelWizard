namespace CT_MKWII_WPF.Services;

public static class Endpoints
{
    // Retro Rewind
    public const string RRUrl = "http://update.zplwii.xyz:8000/";
    public const string RRZipUrl = RRUrl + "RetroRewind/zip/RetroRewind.zip";
    public const string RRVersionUrl = RRUrl + "RetroRewind/RetroRewindVersion.txt";
    
    // WheelWizard
    public const string WhWzStatusUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/status.txt";
    public const string WhWzVersionUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/version.txt";
    public const string WhWzLatestReleasedUrl = "https://github.com/patchzyy/WheelWizard/releases/latest/download/WheelWizard.exe";
    public const string WhWzDiscordUrl = "https://discord.gg/vZ7T2wJnsq";
    public const string WhWzGithubUrl = "https://github.com/patchzyy/WheelWizard";
    
    // Other
    public const string MiiStudioUrl = "https://qrcode.rc24.xyz/cgi-bin/studio.cgi";
    public const string MiiImageUrl = "https://studio.mii.nintendo.com/miis/image.png";

}