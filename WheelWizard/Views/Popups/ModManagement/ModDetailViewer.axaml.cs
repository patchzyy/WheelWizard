using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WheelWizard.Helpers;
using WheelWizard.Models.GameBanana;
using WheelWizard.Services;
using WheelWizard.Services.GameBanana;
using WheelWizard.Services.Installation;
using WheelWizard.Services.Launcher;
using WheelWizard.WPFViews.Popups.Generic;
using MessageBoxWindow = WheelWizard.Views.Popups.Generic.MessageBoxWindow;

namespace WheelWizard.Views.Popups.ModManagment;

public partial class ModDetailViewer : UserControl
{
    private ModDetailResponse CurrentMod { get; set; }
    
    public ModDetailViewer()
    {
        InitializeComponent();
        this.IsVisible = false;
        UnInstallButton.IsVisible = false;
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
                new MessageBoxWindow().SetMainText($"Failed to retrieve mod details: {modDetailsResult.StatusMessage}").Show();
                return false;
            }
            var modDetails = modDetailsResult.Content;
            ImageCarousel.Items.Clear();
            var firstImage = true;
            if (modDetails._aPreviewMedia?._aImages != null && modDetails._aPreviewMedia._aImages.Any())
            {
                foreach (var fullImageUrl in modDetails._aPreviewMedia._aImages.Select(image => $"{image._sBaseUrl}/{image._sFile}"))
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(fullImageUrl);
                    response.EnsureSuccessStatusCode();
                    
                    await using var stream = await response.Content.ReadAsStreamAsync();
                    var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    var bitmap = new Bitmap(memoryStream);
                    
                    ImageCarousel.Items.Add(new { FullImageUrl = bitmap });
                    if (firstImage)
                    {
                        firstImage = false;
                        BannerImage.Source = bitmap;
                    }
                }
            }

            ModTitle.Text = modDetails._sName;
            AuthorButton.Text = modDetails._aSubmitter._sName;
            
            LikesCountBox.Text = modDetails._nLikeCount.ToString();
            ViewsCountBox.Text = modDetails._nViewCount.ToString();
            DownloadsCountBox.Text = modDetails._nDownloadCount.ToString();
           
            // IMPORTANT: the text has to be in a div tag. Since otherwise we cant apply any style to the text that has not tags around it
            ModDescriptionTextBlock.Text = modDetails._sText;

            CurrentMod = modDetails;
            CurrentMod.OverrideDownloadUrl = newDownloadUrl;
            UpdateDownloadButtonState(ModId);
            IsVisible = true;
        }
        catch (Exception ex)
        {
            new MessageBoxWindow().SetMainText("An error occurred while fetching mod details: " + ex.Message).Show();
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
            UnInstallButton.IsVisible = true;
            return;
        }
        
        InstallButton.Content = "Download and Install";
        InstallButton.IsEnabled = true; // Enable button if not installed
        UnInstallButton.IsVisible = false;
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
        ModDescriptionTextBlock.Text = string.Empty;
        IsVisible = false; 
    }
    
    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = new Views.Popups.Generic.YesNoWindow().SetMainText($"Do you want to download and install the mod: {CurrentMod._sName}?").AwaitAnswer();
        if (!confirmation)
        {
            new MessageBoxWindow().SetMainText("Download cancelled.").Show();
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
                new MessageBoxWindow().SetMainText("No downloadable files found for this mod.").Show();
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
                new MessageBoxWindow().SetMainText("Downloaded file not found.").Show();
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
                new MessageBoxWindow().SetMainText("Mod name not provided.").Show();
                return;
            }
            var invalidChars = Path.GetInvalidFileNameChars();
            if (modName.Any(c => invalidChars.Contains(c)))
            {
                new MessageBoxWindow().SetMainText("Mod name contains invalid characters.").Show();
                Directory.Delete(ModsLaunchHelper.TempModsFolderPath, true);
                return;
            }
            await ModInstallation.InstallModFromFileAsync(file, modName ,author, modId);
            Directory.Delete(ModsLaunchHelper.TempModsFolderPath, true);
        }
        catch (Exception ex)
        {
            new MessageBoxWindow().SetMainText("An error occurred during download: " + ex.Message).Show();
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
        IsVisible = false;
    }

    private void AuthorLink_Click(object? sender, EventArgs eventArgs)
    {
        var profileURL = CurrentMod._aSubmitter._sProfileUrl;
        ViewUtils.OpenLink(profileURL);
    }

    private void GamebananaLink_Click(object? sender, EventArgs eventArgs)
    {
        ViewUtils.OpenLink(CurrentMod._sProfileUrl);
    }
    private void ReportLink_Click(object? sender, EventArgs eventArgs)
    {
        var url = $"https://gamebanana.com/support/add?s=Mod.{CurrentMod._idRow}";
        ViewUtils.OpenLink(url);
    }
    
    private void UnInstall_Click(object sender, RoutedEventArgs e)
    {
        ModManager.Instance.DeleteModById(CurrentMod._idRow);
        UpdateDownloadButtonState(CurrentMod._idRow);
    }
}
