using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;

namespace CT_MKWII_WPF.Services.Settings;

// TODO: 
// - Add a new Setting type "DolphinSetting" which has 2 extra names, the FileName and the SectionName
// - Test this new Setting type thoroughly with a fake ini file before trying it on the real data
// - Add a new Setting type "VirtualSetting", this setting will not actually save any data, but has a lambda for getting and setting data
//      This is e.g. used for the Recommended setting,  which instead gets and sets multiple DolphinSettings
//      Do a little research before you implement this, since maybe you dont even have to do this using lambda's,
//      but instead yuo could create another class like "GroupSetting" which just has a bunch of Settings and their value on both toggles
//      However, if you make such a setting, it should probably be limited to bools and enums.
//      You probably just need to make a Virtual setting and extend that with this GroupSeting that only allows bools and enums

public class SettingsManager
{
    public static Setting USER_FOLDER_PATH = new WhWzSetting(typeof(string),"UserFolderPath", "").SetValidation(value => FileHelper.DirectoryExists(value as string ?? string.Empty));
    public static Setting DOLPHIN_LOCATION = new WhWzSetting(typeof(string),"DolphinLocation", "").SetValidation(value => FileHelper.FileExists(value as string ?? string.Empty));
    public static Setting GAME_LOCATION = new WhWzSetting(typeof(string),"GameLocation", "").SetValidation(value => FileHelper.FileExists(value as string ?? string.Empty));
    public static Setting FORCE_WIIMOTE = new WhWzSetting(typeof(bool),"ForceWiimote", false);

    // public static Setting DOLHPIN_CORE = new DolphinSetting(typeof(string), ("test.ini", "enum", "eeeee"), "hey").SetValidation(value => ((string)value!) != "");
    
    // dont ever make this a static class, it is required to be an instance class to ensure all settings are loaded
    public static SettingsManager Instance { get; } = new();
    private SettingsManager() { }
    
    public void LoadSettings()
    {
        WhWzSettingManager.Instance.LoadSettings();
        DolphinSettingManager.Instance.LoadSettings();
    }
}
