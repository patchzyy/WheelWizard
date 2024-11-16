using WheelWizard.Services.Settings;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WheelWizard.Helpers;
using WheelWizard.Models.Settings;
using WheelWizard.Resources.Languages;
using WheelWizard.WPFViews.Popups.Generic;

namespace WheelWizard.Services.Launcher;

public static class ModsLaunchHelper
{
    public static readonly string[] AcceptedModExtensions = { "*.szs", "*.arc", "*.brstm", "*.brsar", "*.thp" };

    public static string MyStuffFolderPath => Path.Combine(PathManager.RetroRewind6FolderPath, "MyStuff");
    public static string ModsFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods");
    public static string TempModsFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", "Temp");
    
    public static async Task PrepareModsForLaunch()
    {
        var mods = ModManager.Instance.Mods.Where(mod => mod.IsEnabled).ToArray();
        if (mods.Length == 0)
        {
            if (Directory.Exists(MyStuffFolderPath) && Directory.EnumerateFiles(MyStuffFolderPath).Any())
            {
                var modsFoundQuestion = new YesNoWindow()
                                        .SetButtonText(Common.Action_Delete, Common.Action_Keep)
                                        .SetMainText(Phrases.PopupText_ModsFound)
                                        .SetExtraText(Phrases.PopupText_ModsFoundQuestion);
                if (modsFoundQuestion.AwaitAnswer())
                    Directory.Delete(MyStuffFolderPath, true);
                
                return;
            }
        }
        var reversedMods = ModManager.Instance.Mods.Reverse().ToArray();
        
        if (Directory.Exists(MyStuffFolderPath))
            Directory.Delete(MyStuffFolderPath, true);
        
        
        var progressWindow = new ProgressWindow(Phrases.PopupText_InstallingMods)
            .SetGoal(Humanizer.ReplaceDynamic(Phrases.PopupText_InstallingModsCount, mods.Length)!);
        progressWindow.Show();
        
        await Task.Run(() =>
        {
            foreach (var mod in reversedMods)
            {
                if (mod.IsEnabled) 
                    InstallMod(mod, progressWindow);
            }
        });
        progressWindow.Close();
    }
    
    private static void InstallMod(Mod mod, ProgressWindow progressPopupWindow)
    {
              
        var modFolder = Path.Combine(ModsFolderPath, mod.Title);
        var allFiles = Array.Empty<string>();
        //important to use Dispatcher otherwise program has thread issues or smth
        Application.Current.Dispatcher.Invoke(() =>
            progressPopupWindow.SetExtraText($"{Common.State_Installing} {mod.Title} ({allFiles.Length} files)")); 
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
