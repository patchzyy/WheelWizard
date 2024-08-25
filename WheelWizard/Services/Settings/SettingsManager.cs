using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;

namespace CT_MKWII_WPF.Services.Settings;

public class SettingsManager
{
    public static Setting USER_FOLDER_PATH = new WhWzSetting(typeof(string),"UserFolderPath", "").SetValidation(value => FileHelper.DirectoryExists(value as string ?? string.Empty));
    public static Setting DOLPHIN_LOCATION = new WhWzSetting(typeof(string),"DolphinLocation", "").SetValidation(value => FileHelper.FileExists(value as string ?? string.Empty));
    public static Setting GAME_LOCATION = new WhWzSetting(typeof(string),"GameLocation", "").SetValidation(value => FileHelper.FileExists(value as string ?? string.Empty));
    public static Setting FORCE_WIIMOTE = new WhWzSetting(typeof(bool),"ForceWiimote", false);

    public static Setting DOLPHIN1_CORE = new DolphinSetting(typeof(string), ("test.ini", "mySection", "key"), "hello world").SetValidation(value => ((string)value!) != "");
    public static Setting DOLPHIN1_CORE2 = new DolphinSetting(typeof(int), ("test.ini", "mySection", "key2"), 42).SetValidation(value => ((int)value!) >= 0);
    public static Setting DOLPHIN2_CORE = new DolphinSetting(typeof(string), ("test.ini", "mySection2", "key"), "by by world").SetValidation(value => ((string)value!) != "");
    public static Setting DOLPHIN2_CORE2 = new DolphinSetting(typeof(int), ("test.ini", "mySection2", "key2"), 0).SetValidation(value => ((int)value!) >= 0);

    public static Setting VIRTUAL_TEST = new VirtualSetting(typeof(bool), value => {
                                                                var newValue = (bool)value!;
                                                                DOLPHIN1_CORE.Set(newValue ? "hello world" : "by by world");
                                                                DOLPHIN1_CORE2.Set(newValue ? 42 : 0, true);
                                                                DOLPHIN2_CORE.Set(newValue ? "by by world" : "hello world");
                                                                DOLPHIN2_CORE2.Set(newValue ? 0 : 42);
                                                            },
                                                            () => {
                                                                var value1 = DOLPHIN1_CORE.Get();
                                                                var value2 = DOLPHIN1_CORE2.Get();
                                                                var value3 = DOLPHIN2_CORE.Get();
                                                                var value4 = DOLPHIN2_CORE2.Get();
                                                                var firstCorrect =  value1?.ToString() == "hello world" && (int)value2! == 42;
                                                                var secondCorrect = value3?.ToString() == "by by world" && (int)value4! == 0;   
                                                                return firstCorrect && secondCorrect;
                                                            }).SetDependencies(DOLPHIN1_CORE, DOLPHIN1_CORE2, DOLPHIN2_CORE, DOLPHIN2_CORE2);
    
    // dont ever make this a static class, it is required to be an instance class to ensure all settings are loaded
    public static SettingsManager Instance { get; } = new();
    private SettingsManager() { }
    
    public void LoadSettings()
    {
        WhWzSettingManager.Instance.LoadSettings();
        DolphinSettingManager.Instance.LoadSettings();
    }
}
