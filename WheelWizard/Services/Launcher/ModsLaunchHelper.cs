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
                var modsFoundQuestion = new YesNoWindow()
                                        .SetButtonText("Delete", "Keep")
                                        .SetMainText("Mods found")
                                        .SetExtraText("you are about to launch the game without any mods. Do you want to clear your my-stuff folder?");
                if (modsFoundQuestion.AwaitAnswer())
                    Directory.Delete(MyStuffFolderPath, true);
                
                return;
            }
        }
        
        Array.Reverse(mods);
        if (Directory.Exists(MyStuffFolderPath))
            Directory.Delete(MyStuffFolderPath, true);
        var progressWindow = new ProgressWindow("Installing Mods").SetGoal($"Installing {mods.Length} mods");
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
    
    private static void InstallMod(ModData mod, ProgressWindow progressPopupWindow)
    {
              
        var modFolder = Path.Combine(ModsFolderPath, mod.Title);
        var allFiles = Array.Empty<string>();
        progressPopupWindow.SetExtraText( $"Installing {mod.Title} ({allFiles.Length} files)" );
        
        foreach (var extension in AcceptedModExtensions)
        {
            var files = Directory.GetFiles(modFolder, extension, SearchOption.AllDirectories);
            allFiles = allFiles.Concat(files).ToArray();
        }

        for (var i = 0; i < allFiles.Length; i++)
        {
            var file = allFiles[i];
            var progress = (int)((i + 1) / (double)allFiles.Length * 100);
            Application.Current.Dispatcher.Invoke(() => progressPopupWindow.UpdateProgress(progress));
            var relativePath = Path.GetFileName(file);
            var destinationFile = Path.Combine(MyStuffFolderPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
            File.Copy(file, destinationFile, true);
        }
    }
}
