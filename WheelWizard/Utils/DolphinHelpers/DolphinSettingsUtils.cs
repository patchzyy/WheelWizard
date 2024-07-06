using System.Windows.Forms;
using CT_MKWII_WPF.Utils.DolphinHelpers;

namespace CT_MKWII_WPF.Utils;

public class DolphinSettingsUtils
{
    public static void EnableReccomendedSettings()
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "ShaderCompilationMode", "2");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings","WaitForShadersBeforeStarting", "True" );
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "MSAA", "0x00000002");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "SSAA", "False");
    }
    
    public static void DisableReccommendedSettings()
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "ShaderCompilationMode", "0");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings","WaitForShadersBeforeStarting", "False" );
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "MSAA", "0x00000001");
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "SSAA", "False");
    }
    
    public static bool IsReccommendedSettingsEnabled()
    {
        var GFXFile = SettingsUtils.FindGFXFile();
        if (GFXFile == "")
        {
            return false;
        }
        var ShaderCompilationMode = DolphinSettingHelper.ReadINISetting(GFXFile, "Settings", "ShaderCompilationMode");
        var WaitForShadersBeforeStarting = DolphinSettingHelper.ReadINISetting(GFXFile, "Settings", "WaitForShadersBeforeStarting");
        var MSAA = DolphinSettingHelper.ReadINISetting(GFXFile, "Settings", "MSAA");
        var SSAA = DolphinSettingHelper.ReadINISetting(GFXFile, "Settings", "SSAA");
        if (ShaderCompilationMode == "2" && WaitForShadersBeforeStarting == "True" && MSAA == "0x00000002" && SSAA == "False")
        {
            return true;
        }
        return false;
    }
    
    public static bool GetCurrentVSyncStatus()
    {
        var GFXFile = SettingsUtils.FindGFXFile();
        if (GFXFile == "")
        {
            //this should NEVER happen, but just in case
            MessageBox.Show("Something went wrong, please contact us via discord. \nGFX file not found");
            return false;
        }
        var VSync = DolphinSettingHelper.ReadINISetting(GFXFile,  "VSync");
        return VSync == "True";
    }
    
    public static int GetCurrentResolution()
    {
        var GFXFile = SettingsUtils.FindGFXFile();
        if (GFXFile == "")
        {
            return -1;
        }
        var resolution = DolphinSettingHelper.ReadINISetting(GFXFile, "Settings", "InternalResolution");
        if (!int.TryParse(resolution, out _)) return -1;
        return int.Parse(resolution);
    }
}