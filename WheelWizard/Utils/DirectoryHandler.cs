using System;
using System.IO;
using System.Linq;

namespace CT_MKWII_WPF.Utils;

public class DirectoryHandler
{
    public static void BackupRR()
    {
        //check if RR is active in the load folder
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        if (Directory.Exists(Path.Combine(LoadPath, "Riivolution", "RetroRewind6")))
        {
            //move the entire Riivolution main folder to Riivolution-RR
            Directory.Move(Path.Combine(LoadPath, "Riivolution"), Path.Combine(LoadPath, "Riivolution-RR"));
        }
    }
    
    public static void RetrieveRR()
    {
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        if (Directory.Exists(Path.Combine(LoadPath, "Riivolution-RR")))
        {
            //move the entire Riivolution-RR folder to Riivolution
            Directory.Move(Path.Combine(LoadPath, "Riivolution-RR"), Path.Combine(LoadPath, "Riivolution"));
        }
    }
    
    public static void BackupRiivolution()
    {
        //first check if Riivolution is active in the load folder, but make sure we dont back up if RR is active
        
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        bool isRiiActive = isRiivolutionActive();
        if (isRiiActive)
        {
            //move the entire Riivolution folder to Riivolution-OG
            Directory.Move(Path.Combine(LoadPath, "Riivolution"), Path.Combine(LoadPath, "Riivolution-OG"));
        }

    }
    
    public static bool isRRActive()
    {
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        return Directory.Exists(Path.Combine(LoadPath, "Riivolution", "RetroRewind6"));
    }
    
    public static bool isRiivolutionActive()
    {
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        //only return true if there is NO RetroRewind6 folder in the Riivolution folder AND the Riivolution folder exists
        return !Directory.Exists(Path.Combine(LoadPath, "Riivolution", "RetroRewind6")) && Directory.Exists(Path.Combine(LoadPath, "Riivolution"));
        
    }
    
    public static void RetrieveRiivolution()
    {
        var LoadPath = SettingsUtils.GetLoadPathLocation();
        if (Directory.Exists(Path.Combine(LoadPath, "Riivolution-OG")))
        {
            //move the entire Riivolution-OG folder to Riivolution
            Directory.Move(Path.Combine(LoadPath, "Riivolution-OG"), Path.Combine(LoadPath, "Riivolution"));
        }
    }

    public static void InstallMod(ModData mod)
    {
        // Find the mod folder inside of the appdata mods folder
        var modFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", mod.Title);
    
        // Find the destination folder
        var destinationFolder = Path.Combine(SettingsUtils.GetLoadPathLocation(), "Riivolution", "RetroRewind6", "MyStuff");

        // Get all files with .szs and .brmstm extensions in the mod folder and its subfolders
        var szsFiles = Directory.GetFiles(modFolder, "*.szs", SearchOption.AllDirectories);
        var brmstmFiles = Directory.GetFiles(modFolder, "*.brstm", SearchOption.AllDirectories);
    
        // Create a combined list of all the files
        var allFiles = szsFiles.Concat(brmstmFiles).ToArray();

        foreach (var file in allFiles)
        {
            var relativePath = Path.GetFileName(file); 
            var destinationFile = Path.Combine(destinationFolder, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
            File.Copy(file, destinationFile, true);
        }
    }
}