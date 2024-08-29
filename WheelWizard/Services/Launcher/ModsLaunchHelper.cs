using CT_MKWII_WPF.Models;
using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Services.Launcher;

public static class ModsLaunchHelper
{
    public static readonly string[] AcceptedModExtensions = { "*.szs", "*.arc", "*.brstm", "*.brsar", "*.thp" };

    private static string MyStuffFolderPath => Path.Combine(PathManager.RetroRewind6FolderPath, "MyStuff");
    private static string ModsFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods");
    
    public static async Task PrepareModsForLaunch()
    {
        var mods = ModConfigManager.GetMods();
        if (mods.Length == 0 || !mods.Any(mod => mod.IsEnabled))
        {
            if (Directory.Exists(MyStuffFolderPath) && Directory.EnumerateFiles(MyStuffFolderPath).Any())
            {
                var result = YesNoMessagebox.Show("Mods found", "Delete","Keep", "you are about to launch the game without any mods", "Do you want to clear your my-stuff folder?");
                if (result)
                    Directory.Delete(MyStuffFolderPath, true);
                
                return;
            }
        }
        
        Array.Reverse(mods);
        if (Directory.Exists(MyStuffFolderPath))
            Directory.Delete(MyStuffFolderPath, true);
        var progressWindow = new ProgressWindow();
        progressWindow.Show();
        await Task.Run(() =>
        {
            foreach (var mod in mods)
            {
                Console.WriteLine("working on mod: " + mod.Title);
                if (mod.IsEnabled) 
                    InstallMod(mod, progressWindow);
            }
        });
        progressWindow.Close();
    }
    
    private static void InstallMod(ModData mod, ProgressWindow progressWindow)
    {
        Application.Current.Dispatcher.Invoke(() => 
            progressWindow.UpdateProgress(0, $"Installing {mod.Title}", "Please wait..."));
        var modFolder = Path.Combine(ModsFolderPath, mod.Title);
        var allFiles = Array.Empty<string>();
        foreach (var extension in AcceptedModExtensions)
        {
            var files = Directory.GetFiles(modFolder, extension, SearchOption.AllDirectories);
            allFiles = allFiles.Concat(files).ToArray();
        }

        for (var i = 0; i < allFiles.Length; i++)
        {
            var file = allFiles[i];
            var progress = (int)((i + 1) / (double)allFiles.Length * 100);
            Application.Current.Dispatcher.Invoke(() => 
                progressWindow.UpdateProgress(progress, $"Installing {mod.Title}", $"Please wait... ({i + 1}/{allFiles.Length})"));            var relativePath = Path.GetFileName(file);
            var destinationFile = Path.Combine(MyStuffFolderPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
            File.Copy(file, destinationFile, true);
        }
    }
}
