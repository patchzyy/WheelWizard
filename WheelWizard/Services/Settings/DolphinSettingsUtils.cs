using CT_MKWII_WPF.Services.IdkWhereThisShouldGo;
using System.Linq;
using System.Windows.Forms;

namespace CT_MKWII_WPF.Services.Settings;

public class DolphinSettingsUtils
{
    private static void ChangeSettings(params (string Key, string Value)[] settings)
    {
        var gfxFile = PathManager.FindGfxFile();
        if (string.IsNullOrEmpty(gfxFile)) return;

        foreach (var (key, value) in settings)
        {
            DolphinSettingHelper.ChangeIniSettings(gfxFile, "Settings", key, value);
        }
    }

    public static void EnableRecommendedSettings()
    {
        ChangeSettings(("ShaderCompilationMode", "2"),
            ("WaitForShadersBeforeStarting", "True"),
            ("MSAA", "0x00000002"),
            ("SSAA", "False")
        );
    }

    public static void DisableRecommendedSettings()
    {
        ChangeSettings(("ShaderCompilationMode", "0"),
            ("WaitForShadersBeforeStarting", "False"),
            ("MSAA", "0x00000001"),
            ("SSAA", "False")
        );
    }

    public static bool IsRecommendedSettingsEnabled()
    {
        var gfxFile = PathManager.FindGfxFile();
        if (string.IsNullOrEmpty(gfxFile)) return false;

        var settings = new[] { "ShaderCompilationMode", "WaitForShadersBeforeStarting", "MSAA", "SSAA" };
        var expectedValues = new[] { "2", "True", "0x00000002", "False" };

        return settings.Select((setting, index) =>
                DolphinSettingHelper.ReadIniSetting(gfxFile, "Settings", setting) == expectedValues[index])
            .All(result => result);
    }

    public static bool GetCurrentVSyncStatus()
    {
        var gfxFile = PathManager.FindGfxFile();
        if (gfxFile != "") return DolphinSettingHelper.ReadIniSetting(gfxFile, "VSync") == "True";
        MessageBox.Show("Something went wrong, please contact us via discord. \nGFX file not found");
        return false;
    }

    public static int GetCurrentResolution()
    {
        var gfxFile = PathManager.FindGfxFile();
        if (gfxFile == "")
        {
            return -1;
        }

        var resolution = DolphinSettingHelper.ReadIniSetting(gfxFile, "Settings", "InternalResolution");
        if (!int.TryParse(resolution, out _)) return -1;
        return int.Parse(resolution);
    }
}
