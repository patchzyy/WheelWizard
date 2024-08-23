using CT_MKWII_WPF.Services.Settings;
using System;
using System.IO;

namespace CT_MKWII_WPF.Services;

public static class PathManager
{
    // IMPORTANT: To keep things consistent all paths should be Attrib expressions,
    //            and either end with `FilePath` or `FolderPath`
    
    // pats set by the user
    public static string GameFilePath => ConfigManager.GetConfig().GameLocation!;
    public static string DolphinFilePath => ConfigManager.GetConfig().DolphinLocation!;
    public static string UserFolderPath => ConfigManager.GetConfig().UserFolderPath!;
    
    // Wheel wizard's appdata paths  (dont have to be expressions since they dont depend on user input like the others)
    public static readonly string WheelWizardAppdataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII");
    public static readonly string WheelWizardConfigFilePath = Path.Combine(WheelWizardAppdataPath, "config.json");
    
    // helper paths for folders used across multiple files
    public static string RiivolutionWhWzFolderPath => Path.Combine(LoadFolderPath, "Riivolution", "WheelWizard");
    public static string RetroRewind6FolderPath => Path.Combine(RiivolutionWhWzFolderPath, "RetroRewind6");
    public static string LoadFolderPath => Path.Combine(UserFolderPath, "Load");
    public static string ConfigFolderPath => Path.Combine(UserFolderPath, "Config");
    public static string WiiFolderPath => Path.Combine(UserFolderPath, "Wii");
}
