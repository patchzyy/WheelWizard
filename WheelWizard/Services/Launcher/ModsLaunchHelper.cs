using CT_MKWII_WPF.Models;
using CT_MKWII_WPF.Services.Settings;
using System;
using System.IO;
using System.Linq;

namespace CT_MKWII_WPF.Services.Launcher;

public static class ModsLaunchHelper
{
    public static readonly string[] AcceptedModExtensions = { "*.szs", "*.arc", "*.brstm", "*.brsar", "*.thp" };

    private static string MyStuffFolderPath => Path.Combine(PathManager.LoadFolderPath, "Riivolution", "RetroRewind6", "MyStuff");
    private static string ModsFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods");
    
    public static void PrepareModsForLaunch()
    {
        var mods = ConfigValidator.GetMods();
        if (mods.Length == 0) return;
        
        Array.Reverse(mods);
        
        if (Directory.Exists(MyStuffFolderPath))
            Directory.Delete(MyStuffFolderPath, true);
        foreach (var mod in mods)
        {
            if (mod.IsEnabled)
                InstallMod(mod);
        }
    }
    
    private static void InstallMod(ModData mod)
    {
        var modFolder = Path.Combine(ModsFolderPath, mod.Title);

        var allFiles = Array.Empty<string>();
        foreach (var extension in AcceptedModExtensions)
        {
            var files = Directory.GetFiles(modFolder, extension, SearchOption.AllDirectories);
            allFiles = allFiles.Concat(files).ToArray();
        }

        foreach (var file in allFiles)
        {
            var relativePath = Path.GetFileName(file);
            var destinationFile = Path.Combine(MyStuffFolderPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
            File.Copy(file, destinationFile, true);
        }
    }
}
