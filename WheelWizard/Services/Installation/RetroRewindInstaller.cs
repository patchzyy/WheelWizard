﻿using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Services.Installation;

public static class RetroRewindInstaller
{
    private static string RetroRewindVersionFilePath => Path.Combine(PathManager.RetroRewind6FolderPath, "version.txt");
    
    public static bool IsRetroRewindInstalled() => File.Exists(RetroRewindVersionFilePath);

    public static string CurrentRRVersion()
    {
        var versionFilePath = RetroRewindVersionFilePath;
        return IsRetroRewindInstalled() ? File.ReadAllText(versionFilePath).Trim() : "Not Installed";
    }

    public static async Task<bool> HandleNotInstalled()
    {
        var result = MessageBox.Show(
            Phrases.PopupText_RRNotDeterment,
            Phrases.PopupText_DownloadRR, MessageBoxButton.YesNo);

        if (result == MessageBoxResult.No) return false;

        await InstallRetroRewind();
        return true;
    }

    public static async Task<bool> HandleOldVersion()
    {
        var result = MessageBox.Show(
            Phrases.PopupText_RRToOld,
            Phrases.PopupText_ReinstallRR, MessageBoxButton.YesNo);

        if (result == MessageBoxResult.No) return false;

        await InstallRetroRewind();
        return true;
    }

    public static async Task InstallRetroRewind()
    {
        if (IsRetroRewindInstalled())
            await HandleReinstall();

        if (hasOldrksys())
        {
            var rksysQuestion = new YesNoWindow()
                                .SetMainText(Phrases.PopupText_OldRksysFound)
                                .SetExtraText(Phrases.PopupText_OldRksysFoundExplained);
            if (rksysQuestion.AwaitAnswer()) 
                await backupOldrksys();

        }
        if (HasOldRR())
        {
            var retroRewindFound = new YesNoWindow()
                                .SetMainText(Phrases.PopupText_OldRRFound)
                                .SetExtraText(Phrases.PopupText_OldRRFoundExplained);
            if (retroRewindFound.AwaitAnswer()) 
            {
                HandleMovingOldRR();
                return;
            }
        }
        var serverResponse = await HttpClientHelper.GetAsync<string>(Endpoints.RRUrl);
        if (!serverResponse.Succeeded)
        {
            MessageBox.Show(Phrases.PopupText_CouldNotConnectServer, 
                            Common.Term_Error, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var tempZipPath = Path.Combine(PathManager.LoadFolderPath, "Temp", "RetroRewind.zip");
        await DownloadAndExtractRetroRewind(tempZipPath);
    }

    private static async Task HandleReinstall()
    {
        var result = MessageBox.Show(Phrases.PopupText_AlreadyFilesRR,
            Phrases.PopupText_ReinstallRR, MessageBoxButton.YesNo);

        if (result == MessageBoxResult.No) 
            return;
        
        DeleteExistingRetroRewind();
    }

    private static void HandleMovingOldRR()
    {
        var oldRRFolder = Path.Combine(PathManager.LoadFolderPath, "Riivolution", "RetroRewind6");
        var RRXml = Path.Combine(PathManager.LoadFolderPath, "Riivolution", "riivolution", "RetroRewind6.xml");
        var RRXmlPath = Path.Combine(PathManager.RiivolutionWhWzFolderPath, "riivolution");
        Directory.CreateDirectory(RRXmlPath);
        var RRXmlFile = Path.Combine(RRXmlPath, "RetroRewind6.xml");
        if (File.Exists(RRXmlFile)) 
            File.Delete(RRXmlFile);
        
        File.Move(RRXml, RRXmlFile);
        var newRRFolder = PathManager.RetroRewind6FolderPath;
        if (Directory.Exists(newRRFolder)) 
            Directory.Delete(newRRFolder, true);
        
        Directory.Move(oldRRFolder, newRRFolder);
       
    }
    public static bool HasOldRR()
    {
        var oldRRFolder = Path.Combine(PathManager.LoadFolderPath, "Riivolution", "RetroRewind6");
        var oldRRXml = Path.Combine(PathManager.LoadFolderPath, "Riivolution", "riivolution", "RetroRewind6.xml");
        return Directory.Exists(oldRRFolder) && File.Exists(oldRRXml);
    }

    private static async Task DownloadAndExtractRetroRewind(string tempZipPath)
    {
        var progressWindow = new ProgressWindow(Phrases.PopupText_InstallingRR);
        progressWindow.SetExtraText(Phrases.PopupText_InstallingRRFirstTime);
        progressWindow.Show();

        try
        {
            await DownloadHelper.DownloadToLocation(Endpoints.RRZipUrl, tempZipPath, progressWindow);
            progressWindow.SetExtraText(Common.State_Extracting);
            var extractionPath = PathManager.RiivolutionWhWzFolderPath;
            ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);
        }
        finally
        {
            progressWindow.Close();
            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);
        }
    }

    private static bool hasOldrksys()
    {
        var rrWfc = Path.Combine(PathManager.LoadFolderPath, "Riivolution", "riivolution", "save", "RetroWFC");
        if (!Directory.Exists(rrWfc)) 
            return false;
        
        var rksysFiles = Directory.GetFiles(rrWfc, "rksys.dat", SearchOption.AllDirectories);
        return rksysFiles.Length > 0;
    }

    private static async Task backupOldrksys()
    {
        var rrWfc = Path.Combine(PathManager.LoadFolderPath, "Riivolution", "riivolution", "save", "RetroWFC");
        if (!Directory.Exists(rrWfc)) return;
        var rksysFiles = Directory.GetFiles(rrWfc, "rksys.dat", SearchOption.AllDirectories);
        if (rksysFiles.Length == 0) return;
        var sourceFile = rksysFiles[0];
        var regionFolder = Path.GetDirectoryName(sourceFile);
        var regionFolderName = Path.GetFileName(regionFolder);
        var datFileData = await File.ReadAllBytesAsync(sourceFile);
        if (regionFolderName == null) return;
        var destinationFolder = Path.Combine(PathManager.RiivolutionWhWzFolderPath, "riivolution", "save", "RetroWFC", regionFolderName);
        Directory.CreateDirectory(destinationFolder);
        var destinationFile = Path.Combine(destinationFolder, "rksys.dat");
        await File.WriteAllBytesAsync(destinationFile, datFileData);
        
    }

    private static void DeleteExistingRetroRewind()
    {
        var retroRewindPath = PathManager.RiivolutionWhWzFolderPath;
        if (Directory.Exists(retroRewindPath))
            Directory.Delete(retroRewindPath, true);
    }
}
