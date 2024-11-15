using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Services.Launcher;

public static class ModsLaunchHelper
{
    public static readonly string[] AcceptedModExtensions = { "*.szs", "*.arc", "*.brstm", "*.brsar", "*.thp" };

    public static string MyStuffFolderPath => Path.Combine(PathManager.RetroRewind6FolderPath, "MyStuff");
    public static string ModsFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods");
    public static string TempModsFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", "Temp");
    
    
public static async Task PrepareModsForLaunch()
    {
        var enabledMods = ModManager.Instance.Mods.Where(mod => mod.IsEnabled).ToArray();
        if (enabledMods.Length == 0)
        {
            if (Directory.Exists(MyStuffFolderPath) && Directory.EnumerateFiles(MyStuffFolderPath).Any())
            {
                var modsFoundQuestion = new YesNoWindow()
                                        .SetButtonText(Common.Action_Delete, Common.Action_Keep)
                                        .SetMainText(Phrases.PopupText_ModsFound)
                                        .SetExtraText(Phrases.PopupText_ModsFoundQuestion);
                if (modsFoundQuestion.AwaitAnswer()) // Assuming AwaitAnswer is async
                    Directory.Delete(MyStuffFolderPath, true);

                return;
            }
        }
        
        var allModFiles = enabledMods
            .SelectMany(mod => GetModFiles(mod))
            .ToDictionary(file => Path.GetRelativePath(ModsFolderPath, file), file => file);
        
        if (Directory.Exists(MyStuffFolderPath))
        {
            var existingFiles = Directory.GetFiles(MyStuffFolderPath, "*.*", SearchOption.AllDirectories);
            foreach (var existingFile in existingFiles)
            {
                var relativePath = Path.GetRelativePath(MyStuffFolderPath, existingFile);
                if (!allModFiles.ContainsKey(relativePath))
                {
                    File.Delete(existingFile);
                    // Optionally, remove empty directories
                    var directory = Path.GetDirectoryName(existingFile);
                    if (directory != null && !Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        Directory.Delete(directory, true);
                    }
                }
            }
        }
        else
        {
            Directory.CreateDirectory(MyStuffFolderPath);
        }
        
        // Step 3: Show progress window
        var totalFilesToProcess = allModFiles.Count;
        var progressWindow = new ProgressWindow(Phrases.PopupText_InstallingMods)
            .SetGoal(Humanizer.ReplaceDynamic(Phrases.PopupText_InstallingModsCount, totalFilesToProcess));
        progressWindow.Show();

        var processedFiles = 0;

        await Task.Run(() =>
        {
            Parallel.ForEach(allModFiles, kvp =>
            {
                var relativePath = kvp.Key;
                var sourceFile = kvp.Value;
                var destinationFile = Path.Combine(MyStuffFolderPath, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);

                var sourceInfo = new FileInfo(sourceFile);
                var destInfo = File.Exists(destinationFile) ? new FileInfo(destinationFile) : null;

                var shouldCopy = destInfo == null || sourceInfo.Length != destInfo.Length ||
                                  sourceInfo.LastWriteTimeUtc != destInfo.LastWriteTimeUtc;

                if (shouldCopy)
                {
                    File.Copy(sourceFile, destinationFile, true);
                }

                // Update progress
                processedFiles++;
                var progress = (int)((processedFiles / (double)totalFilesToProcess) * 100);
                Application.Current.Dispatcher.Invoke(() => progressWindow.UpdateProgress(progress));
                Application.Current.Dispatcher.Invoke(() =>
                    progressWindow.SetExtraText($"{Common.State_Installing} {relativePath}"));
            });
        });

        progressWindow.Close();
    }
    private static IEnumerable<string> GetModFiles(Mod mod)
    {
        var modFolder = Path.Combine(ModsFolderPath, mod.Title);
        var allFiles = new List<string>();

        foreach (var extension in AcceptedModExtensions)
        {
            if (Directory.Exists(modFolder))
            {
                allFiles.AddRange(Directory.GetFiles(modFolder, "*" + extension, SearchOption.AllDirectories));
            }
        }

        return allFiles;
    }



}
