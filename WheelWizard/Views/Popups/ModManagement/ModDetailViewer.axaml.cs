using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
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
using WheelWizard.Views.Popups.Generic;
using MessageBoxWindow = WheelWizard.Views.Popups.Generic.MessageBoxWindow;

namespace WheelWizard.Views.Popups.ModManagement;

public partial class ModDetailViewer : UserControl
{
    private bool loading;
    private ModDetailResponse? CurrentMod { get; set; }
    
    public ModDetailViewer()
    {
        InitializeComponent();
        ResetVisibility();
        UnInstallButton.IsVisible = false;
    }

    private void ResetVisibility()
    {
        // Method returns false if the details page is not shown
        if (loading)
        {
            LoadingView.IsVisible = true;
            NoDetailsView.IsVisible = false;
            DetailsView.IsVisible = false;
            return;
        }
        if (CurrentMod == null)
        {
            LoadingView.IsVisible = false;
            NoDetailsView.IsVisible = true;
            DetailsView.IsVisible = false;
            return;
        }

        LoadingView.IsVisible = false;
        NoDetailsView.IsVisible = false;
        DetailsView.IsVisible = true;
    }

    /// <summary>
    /// Loads the details of the specified mod into the viewer.
    /// </summary>
    /// <param name="ModId">The ID of the mod to load.</param>
    /// <param name="newDownloadUrl">The download URL to use instead of the one from the mod details.</param>
    public async Task<bool> LoadModDetailsAsync(int ModId, string? newDownloadUrl = null)
    {
        loading = true;
        ResetVisibility();

        var modDetailsResult = await GamebananaSearchHandler.GetModDetailsAsync(ModId);
        if (!modDetailsResult.Succeeded || modDetailsResult.Content == null)
        {
            CurrentMod = null;
            NoDetailsView.Title = "Failed to retrieve mod info";
            NoDetailsView.BodyText = modDetailsResult.StatusMessage ?? "An error occurred while fetching mod details.";
            loading = false;
            ResetVisibility();
            return false;
        }
        
        CurrentMod = modDetailsResult.Content;
      
        // SETTING THE MOD DETAILS
        ModTitle.Text = CurrentMod._sName;
        AuthorButton.Text = CurrentMod._aSubmitter._sName;

        LikesCountBox.Text = CurrentMod._nLikeCount.ToString();
        ViewsCountBox.Text = CurrentMod._nViewCount.ToString();
        DownloadsCountBox.Text = CurrentMod._nDownloadCount.ToString();

        // IMPORTANT: the text has to be in a div tag. Since otherwise we cant apply any style to the text that has not tags around it
        ModDescriptionHtmlPanel.Text = $"<body>{CurrentMod._sText}</body>";
        CurrentMod.OverrideDownloadUrl = newDownloadUrl;
        UpdateDownloadButtonState(ModId);
        
        // IMAGE LOADING
        ImageCarousel.Items.Clear();
        BannerImage.IsVisible = false; // should be false, will be set to true later, we use it as a flag to know if we have to set the first image as the banner
        
        if (CurrentMod._aPreviewMedia?._aImages == null || !CurrentMod._aPreviewMedia._aImages.Any())
        {
            loading = false;
            ResetVisibility();
            return true;
        }
       
        foreach (var fullImageUrl in CurrentMod._aPreviewMedia._aImages.Select(image => $"{image._sBaseUrl}/{image._sFile}"))
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
            
            if (BannerImage.IsVisible) continue;
            // code here only happens the first iteration of the loop
            BannerImage.IsVisible = true;
            BannerImage.Source = bitmap;
            loading = false;
            ResetVisibility();
        }
        return true;
    }
    
    private void UpdateDownloadButtonState(int modId)
    {
        var isInstalled = ModManager.Instance.IsModInstalled(modId);
        InstallButton.Content = isInstalled ? "Installed": "Download and Install";
        InstallButton.IsEnabled = !isInstalled; 
        UnInstallButton.IsVisible = isInstalled;
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
        IsVisible = false; 
    }
    
    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = await new Views.Popups.Generic.YesNoWindow().SetMainText($"Do you want to download and install the mod: {CurrentMod._sName}?").AwaitAnswer();
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
            var popup = new TextInputWindow().setLabelText("Mod Name");
            popup.PopulateText(CurrentMod._sName);
            var modName = await popup.ShowDialog();
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
