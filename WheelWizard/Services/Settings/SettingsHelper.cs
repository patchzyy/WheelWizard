namespace CT_MKWII_WPF.Services.Settings;

public static class SettingsHelper
{
    public static bool PathsSetupCorrectly()
    {
        return SettingsManager.USER_FOLDER_PATH.IsValid() &&
               SettingsManager.DOLPHIN_LOCATION.IsValid() &&
               SettingsManager.GAME_LOCATION.IsValid();
    }
}
