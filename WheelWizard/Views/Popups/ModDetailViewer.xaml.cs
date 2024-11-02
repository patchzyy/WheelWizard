using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Services.GameBanana;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using CT_MKWII_WPF.Views.Popups;
using System.IO;
using System.Diagnostics;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components
{
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
            if (mod == null)
            {
                ClearDetails();
                return;
            }

            try
            {
                // Fetch the mod details using its ID
                var modDetailsResult = await GamebananaSearchHandler.GetModDetailsAsync(mod._idRow);

                if (!modDetailsResult.Succeeded || modDetailsResult.Content == null)
                {
                    MessageBox.Show($"Failed to retrieve mod details: {modDetailsResult.StatusMessage}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var modDetails = modDetailsResult.Content;

                // Load Images
                ImageCarousel.Items.Clear();
                if (modDetails._aPreviewMedia?._aImages != null && modDetails._aPreviewMedia._aImages.Any())
                {
                    foreach (var image in modDetails._aPreviewMedia._aImages)
                    {
                        var fullImageUrl = $"{image._sBaseUrl}/{image._sFile}";
                        ImageCarousel.Items.Add(new { FullImageUrl = fullImageUrl });
                    }
                }

                // Mod Name and Submitter
                ModName.Text = modDetails._sName;
                ModSubmitter.Text = $"By {modDetails._aSubmitter._sName}";

                // Mod Stats
                ModStats.Text =
                    $"Likes: {modDetails._nLikeCount} | Views: {modDetails._nViewCount} | Downloads: {modDetails._nDownloadCount}";

                // Description
                ModDescriptionHtmlPanel.Text = modDetails._sText;

                // Store the current mod for download
                CurrentMod = mod;

                // Make the viewer visible
                this.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching mod details: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                await PrepareToDownloadFile();
                var modDetailResult = await GamebananaSearchHandler.GetModDetailsAsync(CurrentMod._idRow);
                if (!modDetailResult.Succeeded || modDetailResult.Content == null)
                {
                    MessageBox.Show($"Failed to retrieve mod details: {modDetailResult.StatusMessage}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var downloadUrls = modDetailResult.Content._aFiles
                        .Select(f => f._sDownloadUrl)
                        .ToList();
                if (!downloadUrls.Any())
                {
                    MessageBox.Show("No download URLs found for the mod.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                    var progressWindow = new ProgressWindow($"Downloading {CurrentMod._sName}",
                        Application.Current.MainWindow);
                    progressWindow.Show();
                    foreach (var url in downloadUrls)
                    {
                        await PrepareToDownloadFile();
                        await DownloadFileAsync(url, progressWindow);
                    }

                    progressWindow.Close();
                    var file = Directory.GetFiles(ModsLaunchHelper.TempModsFolderPath).FirstOrDefault();
                    if (file == null)
                    {
                            MessageBox.Show("Downloaded file not found.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
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
        }

        /// <summary>
        /// Downloads a file from the specified URL and updates the progress window.
        /// </summary>
        private async Task DownloadFileAsync(string url, ProgressWindow progressWindow)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true }))
            {
                try
                {
                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    // Follow redirects to get the final URL
                    var finalUrl = response.RequestMessage.RequestUri.ToString();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    // Determine the file name
                    var contentDisposition = response.Content.Headers.ContentDisposition;
                    var fileName = contentDisposition?.FileName?.Trim('"') ?? GetFileNameFromUrl(finalUrl);

                    var modsFolder = ModsLaunchHelper.TempModsFolderPath;
                    Directory.CreateDirectory(modsFolder);
                    var filePath = Path.Combine(modsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var httpStream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;
                        while ((bytesRead = await httpStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;
                            if (canReportProgress)
                            {
                                double progress = (double)totalRead / totalBytes * 100;
                                progressWindow.UpdateProgress((int)progress);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to download file from {url}: {ex.Message}");
                }
            }
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
            this.Visibility = Visibility.Collapsed;
        }
    }
}
