using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Enums;
using CT_MKWII_WPF.Models.Settings;
using System;

namespace CT_MKWII_WPF.Services.Settings;

public class SettingsManager
{
    // Wheel Wizard Settings
    public static Setting USER_FOLDER_PATH = new WhWzSetting(typeof(string),"UserFolderPath", "").SetValidation(value => FileHelper.DirectoryExists(value as string ?? string.Empty));
    public static Setting DOLPHIN_LOCATION = new WhWzSetting(typeof(string),"DolphinLocation", "").SetValidation(value => FileHelper.FileExists(value as string ?? string.Empty));
    public static Setting GAME_LOCATION = new WhWzSetting(typeof(string),"GameLocation", "").SetValidation(value => FileHelper.FileExists(value as string ?? string.Empty));
    public static Setting FORCE_WIIMOTE = new WhWzSetting(typeof(bool),"ForceWiimote", false);
    public static Setting LAUNCH_WITH_DOLPHIN = new WhWzSetting(typeof(bool),"LaunchWithDolphin", false);
    public static Setting FOCUSSED_USER = new WhWzSetting(typeof(int), "FavoriteUser", 0); //.SetValidation(value => (int)(value ?? -1) >= 0 && (int)(value ?? -1) <= 4);
    public static Setting FORCE_30FPS = new WhWzSetting(typeof(bool), "Force30FPS", false);

    // Dolphin Settings
    public static Setting VSYNC = new DolphinSetting(typeof(bool), ("GFX.ini", "Hardware", "VSync"), false);

    public static Setting INTERNAL_RESOLUTION = new DolphinSetting(typeof(int), ("GFX.ini", "Settings", "InternalResolution"), 1)
        .SetValidation(value => (int)(value ?? -1) >= 0);
  
    
    public static Setting SHOW_FPS = new DolphinSetting(typeof(bool), ("GFX.ini", "Settings", "ShowFPS"), false);
    public static Setting GFX_BACKEND = new DolphinSetting(typeof(string), ("Dolphin.ini", "Settings", "Backend"), SettingValues.GFXRenderers[0]);
    
    //recommended settings
    private static Setting DOLPHIN_COMPILATION_MODE = new DolphinSetting(typeof(DolphinShaderCompilationMode), ("GFX.ini", "Settings", "ShaderCompilationMode"),
        DolphinShaderCompilationMode.Default);
    private static Setting DOLPHIN_COMPILE_SHADERS_AT_START = new DolphinSetting(typeof(bool), ("GFX.ini", "Settings", "WaitForShadersBeforeStarting"), false);
    private static Setting DOLPHIN_SSAA = new DolphinSetting(typeof(bool), ("GFX.ini", "Settings", "SSAA"), false);
    private static Setting DOLPHIN_MSAA = new DolphinSetting(typeof(string), ("GFX.ini", "Settings", "MSAA"), "0x00000001").SetValidation(
        value => ( value?.ToString() ?? "") is "0x00000001" or "0x00000002" or "0x00000004" or "0x00000008");
    
    // Virtual Settings
    public static Setting RECOMMENDED_SETTINGS = new VirtualSetting(typeof(bool), value => {
                                                                var newValue = (bool)value!;
                                                                DOLPHIN_COMPILATION_MODE.Set(newValue ? DolphinShaderCompilationMode.HybridUberShaders : DolphinShaderCompilationMode.Default);
                                                                DOLPHIN_COMPILE_SHADERS_AT_START.Set(newValue);
                                                                DOLPHIN_MSAA.Set(newValue ? "0x00000002" : "0x00000001");
                                                                DOLPHIN_SSAA.Set(false);
                                                            },
                                                            () => {
                                                                var value1 = (DolphinShaderCompilationMode)DOLPHIN_COMPILATION_MODE.Get();
                                                                var value2 = (bool)DOLPHIN_COMPILE_SHADERS_AT_START.Get();
                                                                var value3 = (string)DOLPHIN_MSAA.Get();
                                                                var value4 = (bool)DOLPHIN_SSAA.Get();
                                                                return !value4 && value2 && value3 == "0x00000002" && value1 == DolphinShaderCompilationMode.HybridUberShaders;
                                                            }).SetDependencies(DOLPHIN_COMPILATION_MODE, DOLPHIN_COMPILE_SHADERS_AT_START, DOLPHIN_MSAA, DOLPHIN_SSAA);
    
    
    // dont ever make this a static class, it is required to be an instance class to ensure all settings are loaded
    public static SettingsManager Instance { get; } = new();
    private SettingsManager() { }
    
    public void LoadSettings()
    {
        WhWzSettingManager.Instance.LoadSettings();
        DolphinSettingManager.Instance.LoadSettings();
    }
}
