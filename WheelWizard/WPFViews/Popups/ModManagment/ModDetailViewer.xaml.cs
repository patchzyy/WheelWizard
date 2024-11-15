using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WheelWizard.Helpers;
using WheelWizard.Models.GameBanana;
using WheelWizard.Services;
using WheelWizard.Services.GameBanana;
using WheelWizard.Services.Installation;
using WheelWizard.Services.Launcher;
using WheelWizard.Views.Popups;

namespace WheelWizard.Views.Components;

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
            var firstImage = true;
            if (modDetails._aPreviewMedia?._aImages != null && modDetails._aPreviewMedia._aImages.Any())
            {
                foreach (var fullImageUrl in modDetails._aPreviewMedia._aImages.Select(image => $"{image._sBaseUrl}/{image._sFile}"))
                {
                    ImageCarousel.Items.Add(new { FullImageUrl = fullImageUrl });
                    if (firstImage)
                    {
                        firstImage = false;
                        BannerImage.Source = new BitmapImage(new Uri(fullImageUrl));
                    }
                }
            }
            
            ModTitle.Text = modDetails._sName;
            AuthorButton.Text = modDetails._aSubmitter._sName;
            
            LikesCountBox.Text = modDetails._nLikeCount.ToString();
            ViewsCountBox.Text = modDetails._nViewCount.ToString();
            DownloadsCountBox.Text = modDetails._nDownloadCount.ToString();
           
            // IMPORTANT: the text has to be in a div tag. Since otherwise we cant apply any style to the text that has not tags around it
            ModDescriptionHtmlPanel.Text = $"<body>{modDetails._sText}</body>";

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
            InstallButton.Content = "Installed";
            InstallButton.IsEnabled = false;
            UnInstallButton.Visibility = Visibility.Visible;
            return;
        }
        
        InstallButton.Content = "Download and Install";
        InstallButton.IsEnabled = true; // Enable button if not installed
        UnInstallButton.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Clears the mod details from the viewer.
    /// </summary>
    private void ClearDetails()
    {
        ImageCarousel.Items.Clear();
        ModTitle.Text = string.Empty;
        AuthorButton.Text = "Unknown";
        LikesCountBox.Text = ViewsCountBox.Text = DownloadsCountBox.Text = "0";
        ModDescriptionHtmlPanel.Text = string.Empty;
        Visibility = Visibility.Collapsed; 
    }
    
    private async void Install_Click(object sender, RoutedEventArgs e)
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
            progressWindow.SetExtraText("Loading...");

            var url = downloadUrls.First();
            var fileName = GetFileNameFromUrl(url);
            var filePath = Path.Combine(ModsLaunchHelper.TempModsFolderPath, fileName);
            await DownloadHelper.DownloadToLocationAsync(url, filePath, progressWindow);
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
            var popup = new TextInputPopup("Enter Mod Name");
            popup.PopulateText(CurrentMod._sName);
            var modName = popup.ShowDialog();
            if (string.IsNullOrEmpty(modName))
            {
                MessageBoxWindow.ShowDialog("Mod name not provided.");
                return;
            }
            await ModInstallation.InstallModFromFileAsync(file, modName ,author, modId);
            Directory.Delete(ModsLaunchHelper.TempModsFolderPath, true);
        }
        catch (Exception ex)
        {
            MessageBoxWindow.ShowDialog("An error occurred during download: " + ex.Message);
        }
        LoadModDetailsAsync(CurrentMod._idRow);
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

    private void AuthorLink_Click(object sender, RoutedEventArgs e)
    {
        var profileURL = CurrentMod._aSubmitter._sProfileUrl;
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = profileURL,
            UseShellExecute = true
        });
    }

    private void GamebananaLink_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = CurrentMod._sProfileUrl,
            UseShellExecute = true
        });
    }
    private void ReportLink_Click(object sender, RoutedEventArgs e)
    {
        var url = $"https://gamebanana.com/support/add?s=Mod.{CurrentMod._idRow}";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    
    private void UnInstall_Click(object sender, RoutedEventArgs e)
    {
        ModManager.Instance.DeleteModById(CurrentMod._idRow);
        UpdateDownloadButtonState(CurrentMod._idRow);
    }
    
    private void ImageScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // This method is for either 1 of these reasons:
        // 1. WPF sucks and there is no real good horizontal scroll view thing
        // OR
        // 2. I am stupid and don't know how to use WPF properly
        if (ImageScrollViewer.ComputedHorizontalScrollBarVisibility != Visibility.Visible) return;

        ImageScrollViewer.ScrollToHorizontalOffset(ImageScrollViewer.HorizontalOffset - e.Delta);
        e.Handled = true;
    }
}

