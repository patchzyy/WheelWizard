﻿using System;
using System.IO;
using System.Linq;
using CT_MKWII_WPF.Models;
using CT_MKWII_WPF.Services.Configuration;

namespace CT_MKWII_WPF.Services.Installation;

public static class DirectoryHandler
{
    public static void InstallMod(ModData mod)
    {
        // Find the mod folder inside the appdata mods folder
        var modFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CT-MKWII",
            "Mods", mod.Title);

        // Find the destination folder
        var destinationFolder =
            Path.Combine(PathManager.GetLoadPathLocation(), "Riivolution", "RetroRewind6", "MyStuff");

        // Get all files with .szs and .brmstm extensions in the mod folder and its subfolders
        var szsFiles = Directory.GetFiles(modFolder, "*.szs", SearchOption.AllDirectories);
        var brstmFiles = Directory.GetFiles(modFolder, "*.brstm", SearchOption.AllDirectories);
        var brsarFiles = Directory.GetFiles(modFolder, "*.brsar", SearchOption.AllDirectories);

        // Create a combined list of all the files
        var allFiles = szsFiles.Concat(brstmFiles).ToArray();
        allFiles = allFiles.Concat(brsarFiles).ToArray();

        foreach (var file in allFiles)
        {
            var relativePath = Path.GetFileName(file);
            var destinationFile = Path.Combine(destinationFolder, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
            File.Copy(file, destinationFile, true);
        }
    }
}