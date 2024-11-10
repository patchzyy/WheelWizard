using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.GameBanana;
using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.GameBanana;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using CT_MKWII_WPF.Views.Popups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Views.Components;

public partial class ModDetailViewer : UserControl
{
    private ModDetailResponse CurrentMod { get; set; }
    public ModDetailViewer()
    {
        InitializeComponent();
        this.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Loads the details of the specified mod into the viewer.
    /// </summary>
    /// <param name="ModId">The ID of the mod to load.</param>
    /// <param name="newDownloadUrl">The download URL to use instead of the one from the mod details.</param>
    public async Task<bool> LoadModDetailsAsync(int ModId, string? newDownloadUrl = null)
    {
        try
        {
            var modDetailsResult = await GamebananaSearchHandler.GetModDetailsAsync(ModId);
            if (!modDetailsResult.Succeeded || modDetailsResult.Content == null)
            {
                MessageBoxWindow.ShowDialog($"Failed to retrieve mod details: {modDetailsResult.StatusMessage}");
                return false;
            }
            var modDetails = modDetailsResult.Content;
            ImageCarousel.Items.Clear();
            if (modDetails._aPreviewMedia?._aImages != null && modDetails._aPreviewMedia._aImages.Any())
            {
                foreach (var fullImageUrl in modDetails._aPreviewMedia._aImages.Select(image => $"{image._sBaseUrl}/{image._sFile}"))
                {
                    ImageCarousel.Items.Add(new { FullImageUrl = fullImageUrl });
                }
            }
            ModName.Text = modDetails._sName;
            ModSubmitter.Text = $"By {modDetails._aSubmitter._sName}";
            ModStats.Text = $"Likes: {modDetails._nLikeCount} | Views: {modDetails._nViewCount} | Downloads: {modDetails._nDownloadCount}";
            ModDescriptionHtmlPanel.Text = modDetails._sText;
            CurrentMod = modDetails;
            CurrentMod.OverrideDownloadUrl = newDownloadUrl;
            UpdateDownloadButtonState(ModId);
            Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            MessageBoxWindow.ShowDialog("An error occurred while fetching mod details: " + ex.Message);
            return false;
        }

        return true;
    }
    
    private void UpdateDownloadButtonState(int modId)
    {
        if (ModManager.Instance.IsModInstalled(modId))
        {
            DownloadButton.Content = "Installed";
            DownloadButton.IsEnabled = false;
            return;
        }
        DownloadButton.Content = "Download and Install";
        DownloadButton.IsEnabled = true; // Enable button if not installed
    }

    /// <summary>
    /// Clears the mod details from the viewer.
    /// </summary>
    private void ClearDetails()
    {
        ImageCarousel.Items.Clear();
        ModName.Text = string.Empty;
        ModSubmitter.Text = string.Empty;
        ModStats.Text = string.Empty;
        ModDescriptionHtmlPanel.Text = string.Empty;
        this.Visibility = Visibility.Collapsed;
    }
    
    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = new YesNoWindow().SetMainText($"Do you want to download and install the mod: {CurrentMod._sName}?").AwaitAnswer();
        if (!confirmation)
        {
            MessageBoxWindow.ShowDialog("Download cancelled.");
            return;
        }
    
        try
        {
            await PrepareToDownloadFile();
    
            var downloadUrls = CurrentMod.OverrideDownloadUrl != null 
                ? new List<string> { CurrentMod.OverrideDownloadUrl }
                : CurrentMod._aFiles.Select(f => f._sDownloadUrl).ToList();
    
            if (!downloadUrls.Any())
            {
                MessageBoxWindow.ShowDialog("No downloadable files found for this mod.");
                return;
            }
    
            var progressWindow = new ProgressWindow($"Downloading {CurrentMod._sName}");
            progressWindow.Show();
    
            foreach (var url in downloadUrls)
            {
                var fileName = GetFileNameFromUrl(url);
                var filePath = Path.Combine(ModsLaunchHelper.TempModsFolderPath, fileName);
                await DownloadHelper.DownloadToLocation(url, filePath, progressWindow);
            }
            progressWindow.Close();
            var file = Directory.GetFiles(ModsLaunchHelper.TempModsFolderPath).FirstOrDefault();
            if (file == null)
            {
                MessageBoxWindow.ShowDialog("Downloaded file not found.");
                return;
            }
            var author = "-1";
            var modId = -1;
            if (CurrentMod._aSubmitter?._sName != null)
            {
                author = CurrentMod._aSubmitter._sName;
            }
            modId = CurrentMod._idRow;
            await ModInstallation.InstallModFromFileAsync(file, author, modId);
            Directory.Delete(ModsLaunchHelper.TempModsFolderPath, true);
        }
        catch (Exception ex)
        {
            MessageBoxWindow.ShowDialog("An error occurred during download: " + ex.Message);
        }
    }


    /// <summary>
    /// Prepares the temporary folder for downloading files.
    /// </summary>
    private static async Task PrepareToDownloadFile()
    {
        var tempFolder = ModsLaunchHelper.TempModsFolderPath;
        if (Directory.Exists(tempFolder))
        {
            Directory.Delete(tempFolder, true);
        }
        Directory.CreateDirectory(tempFolder);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Extracts the file name from a URL.
    /// </summary>
    private static string GetFileNameFromUrl(string url)
    {
        return Path.GetFileName(new Uri(url).AbsolutePath);
    }

    /// <summary>
    /// Clears the mod details and hides the viewer.
    /// </summary>
    public void HideViewer()
    {
        ClearDetails();
        Visibility = Visibility.Collapsed;
    }
}

