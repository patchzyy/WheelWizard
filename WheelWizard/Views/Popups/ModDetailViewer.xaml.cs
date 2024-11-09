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
    public ModDetailViewer()
    {
        InitializeComponent();
        this.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Loads the details of the specified mod.
    /// </summary>
    /// <param name="mod">The mod to display.</param>
    public async Task LoadModDetailsAsync(ModRecord mod, string? newDownloadUrl = null)
    {
        try
        {
            // Fetch the mod details using its ID
            var modDetailsResult = await GamebananaSearchHandler.GetModDetailsAsync(mod._idRow);
            if (!modDetailsResult.Succeeded || modDetailsResult.Content == null)
            {
                MessageBoxWindow.Show($"Failed to retrieve mod details: {modDetailsResult.StatusMessage}");
                return;
            }

            var modDetails = modDetailsResult.Content;

            // Load Images
            ImageCarousel.Items.Clear();
            if (modDetails._aPreviewMedia?._aImages != null && modDetails._aPreviewMedia._aImages.Any())
            {
                foreach (var fullImageUrl in modDetails._aPreviewMedia._aImages.Select(image => $"{image._sBaseUrl}/{image._sFile}"))
                {
                    ImageCarousel.Items.Add(new { FullImageUrl = fullImageUrl });
                }
            }

            // Mod Name and Submitter
            ModName.Text = modDetails._sName;
            ModSubmitter.Text = $"By {modDetails._aSubmitter._sName}";

            // Mod Stats
            ModStats.Text = $"Likes: {modDetails._nLikeCount} | Views: {modDetails._nViewCount} | Downloads: {modDetails._nDownloadCount}";

            // Description
            ModDescriptionHtmlPanel.Text = modDetails._sText;

            // Store the current mod for download
            CurrentMod = modDetails;
            CurrentMod.OverrideDownloadUrl = newDownloadUrl;


            // Update the Download button based on installation status
            UpdateDownloadButtonState(mod._idRow);

            // Make the viewer visible
            this.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            MessageBoxWindow.Show("An error occurred while fetching mod details: " + ex.Message);
        }
    }
    
    private void UpdateDownloadButtonState(int modID)
    {
        if (ModManager.Instance.IsModInstalled(modID))
        {
            DownloadButton.Content = "Installed";
            DownloadButton.IsEnabled = false; // Disable button if already installed
        }
        else
        {
            DownloadButton.Content = "Download and Install";
            DownloadButton.IsEnabled = true; // Enable button if not installed
        }
    }


    /// <summary>
    /// Clears the mod details from the viewer.
    /// </summary>
    public void ClearDetails()
    {
        ImageCarousel.Items.Clear();
        ModName.Text = string.Empty;
        ModSubmitter.Text = string.Empty;
        ModStats.Text = string.Empty;
        ModDescriptionHtmlPanel.Text = string.Empty;
        this.Visibility = Visibility.Collapsed;
    }

    // Property to hold the current mod
    private ModDetailResponse CurrentMod { get; set; }
    
    private async void Download_Click(object sender, RoutedEventArgs e)
{
    var confirmation = new YesNoWindow().SetMainText($"Do you want to download and install the mod: {CurrentMod._sName}?").AwaitAnswer();

    if (!confirmation)
    {
        MessageBoxWindow.Show("Download cancelled.");
        return;
    }

    try
    {
        // Clear temp folder
        await PrepareToDownloadFile();

        var downloadUrls = CurrentMod.OverrideDownloadUrl != null 
            ? new List<string> { CurrentMod.OverrideDownloadUrl }
            : CurrentMod._aFiles.Select(f => f._sDownloadUrl).ToList();

        if (!downloadUrls.Any())
        {
            MessageBoxWindow.Show("No downloadable files found for this mod.");
            return;
        }

        var progressWindow = new ProgressWindow($"Downloading {CurrentMod._sName}");
        progressWindow.Show();

        foreach (var url in downloadUrls)
        {
            // Determine the file name from the URL
            var fileName = GetFileNameFromUrl(url);
            var filePath = Path.Combine(ModsLaunchHelper.TempModsFolderPath, fileName);

            // Use DownloadHelper to download the file
            await DownloadHelper.DownloadToLocation(url, filePath, progressWindow);
        }
        progressWindow.Close();

        // Assuming you install from the first downloaded file
        var file = Directory.GetFiles(ModsLaunchHelper.TempModsFolderPath).FirstOrDefault();
        if (file == null)
        {
            MessageBoxWindow.Show("Downloaded file not found.");
            return;
        }

        // Extract Author and ModID if available
        string author = "-1";
        int modID = -1;
        if (CurrentMod._aSubmitter?._sName != null)
        {
            author = CurrentMod._aSubmitter._sName;
        }

        modID = CurrentMod._idRow;

        await ModInstallation.InstallModFromFileAsync(file, author, modID);
        Directory.Delete(ModsLaunchHelper.TempModsFolderPath, true);
    }
    catch (Exception ex)
    {
        MessageBoxWindow.Show("An error occurred during download: " + ex.Message);
    }
}


    /// <summary>
    /// Prepares the temporary folder for downloading files.
    /// </summary>
    private async Task PrepareToDownloadFile()
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
    private string GetFileNameFromUrl(string url)
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

