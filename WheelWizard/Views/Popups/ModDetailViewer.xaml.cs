using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Services.GameBanana;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using CT_MKWII_WPF.Views.Popups;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Views.Components;

public partial class ModDetailViewer : UserControl
{
    public ModDetailViewer()
    {
        InitializeComponent();
        // Initially hide the detail viewer
        this.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Loads the details of the specified mod.
    /// </summary>
    /// <param name="mod">The mod to display.</param>
    public async Task LoadModDetailsAsync(ModRecord mod)
    {
        try
        {
            // Fetch the mod details using its ID
            var modDetailsResult = await GamebananaSearchHandler.GetModDetailsAsync(mod._idRow);

            if (!modDetailsResult.Succeeded || modDetailsResult.Content == null)
            {
                MessageBox.Show($"Failed to retrieve mod details: {modDetailsResult.StatusMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            CurrentMod = mod;

            // Make the viewer visible
            this.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred while fetching mod details: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    private ModRecord CurrentMod { get; set; }

    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show($"Do you want to download and install the mod: {CurrentMod._sName}?",
                                           "Confirm Download",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);
        
        if (confirmation != MessageBoxResult.Yes)
            return;
        
        try
        {
            // Clear temp folder
            await PrepareToDownloadFile();
        
            // Fetch mod details to get download URLs
            var modDetailResult = await GamebananaSearchHandler.GetModDetailsAsync(CurrentMod._idRow);
            if (!modDetailResult.Succeeded || modDetailResult.Content == null)
            {
                MessageBox.Show("Failed to retrieve mod details.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
        
            var downloadUrls = modDetailResult.Content._aFiles
                                   .Select(f => f._sDownloadUrl)
                                   .ToList();
            if (!downloadUrls.Any())
            {
                MessageBox.Show("No downloadable files found for this mod.",
                                "Info",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
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
                MessageBox.Show("Downloaded file not found.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
            await ModInstallation.InstallModFromFileAsync(file);
            Directory.Delete(ModsLaunchHelper.TempModsFolderPath, true);
            MessageBox.Show("Mod downloaded and installed successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred during download: " + ex.Message,
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
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

