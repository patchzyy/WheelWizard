﻿using System;
using System.IO;
using System.Linq;

namespace CT_MKWII_WPF.Utils;

public class DirectoryHandler
{
    public static void InstallMod(ModData mod)
    {
        // Find the mod folder inside of the appdata mods folder
        var modFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII", "Mods", mod.Title);

        // Find the destination folder
        var destinationFolder = Path.Combine(SettingsUtils.GetLoadPathLocation(), "Riivolution", "RetroRewind6", "MyStuff");

        // Get all files with .szs and .brmstm extensions in the mod folder and its subfolders
        var szsFiles = Directory.GetFiles(modFolder, "*.szs", SearchOption.AllDirectories);
        var arcFiles = Directory.GetFiles(modFolder, "*.arc", SearchOption.AllDirectories);
        var brstmFiles = Directory.GetFiles(modFolder, "*.brstm", SearchOption.AllDirectories);
        var brsarFiles = Directory.GetFiles(modFolder, "*.brsar", SearchOption.AllDirectories);
        var thpFiles = Directory.GetFiles(modFolder, "*.thp", SearchOption.AllDirectories);
        // Create a combined list of all the files
        var allFiles = szsFiles.Concat(arcFiles).Concat(brstmFiles).Concat(brsarFiles).Concat(thpFiles).ToArray();

        foreach (var file in allFiles)
        {
            var relativePath = Path.GetFileName(file);
            var destinationFile = Path.Combine(destinationFolder, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
            File.Copy(file, destinationFile, true);
        }
    }
}