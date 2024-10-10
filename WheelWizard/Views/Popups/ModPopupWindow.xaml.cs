// ModPopupWindow.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models;
using CT_MKWII_WPF.Services.GameBanana;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using System.IO;

namespace CT_MKWII_WPF.Views.Popups
{
    public partial class ModPopupWindow : PopupContent
    {
        private ObservableCollection<ModRecord> Mods { get; set; } = new ObservableCollection<ModRecord>();
        private int CurrentPage { get; set; } = 1;
        private const int ModsPerPage = 20;
        private string CurrentSearchTerm = "";

        public ModPopupWindow() : base(true, false, false, "Mod Browser", new Vector(800, 800))
        {
            InitializeComponent();
            ModListView.ItemsSource = Mods;
            LoadMods(CurrentPage);
        }

        private async void LoadMods(int page, string searchTerm = "")
        {
            try
            {
                var result = await GamebananaSearchHandler.SearchModsAsync(searchTerm, page, ModsPerPage);

                if (result.Succeeded && result.Content != null)
                {
                    Mods.Clear();
                    foreach (var mod in result.Content._aRecords)
                    {
                        Mods.Add(mod);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to load mods: " + result.StatusMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadMods(CurrentPage, CurrentSearchTerm);
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
            LoadMods(CurrentPage, CurrentSearchTerm);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            CurrentSearchTerm = SearchTextBox.Text.Trim();
            CurrentPage = 1;
            LoadMods(CurrentPage, CurrentSearchTerm);
        }

        private void ModListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModListView.SelectedItem is ModRecord selectedMod)
            {
                UpdateModDetails(selectedMod);
            }
        }

        private void UpdateModDetails(ModRecord mod)
        {
            // Load Images
            ImageCarousel.Items.Clear();
            if (mod._aPreviewMedia?._aImages != null && mod._aPreviewMedia._aImages.Any())
            {
                foreach (var image in mod._aPreviewMedia._aImages)
                {
                    var fullImageUrl = $"{image._sBaseUrl}/{image._sFile}";
                    ImageCarousel.Items.Add(new { FullImageUrl = fullImageUrl });
                }
            }

            // Mod Name and Submitter
            ModName.Text = mod._sName;
            ModSubmitter.Text = $"By {mod._aSubmitter._sName}";

            // Mod Stats
            ModStats.Text = $"Likes: {mod._nLikeCount} | Views: {mod._nViewCount}";

            // Description
            ModDescription.Text = mod._sText;
            

            // Development State and Completion
            ModDevelopmentState.Text = mod._sDevelopmentState;
            // CompletionProgressBar.Value = mod._iCompletionPercentage;
        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            if (ModListView.SelectedItem is ModRecord selectedMod)
            {
                var confirmation = MessageBox.Show($"Do you want to download and install the mod: {selectedMod._sName}?",
                                                   "Confirm Download",
                                                   MessageBoxButton.YesNo,
                                                   MessageBoxImage.Question);

                if (confirmation == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Fetch mod details to get download URLs
                        var modDetailResult = await GamebananaSearchHandler.GetModDetailsAsync(selectedMod._idRow);
                        if (modDetailResult.Succeeded && modDetailResult.Content != null)
                        {
                            var downloadUrls = modDetailResult.Content._aFiles.Select(f => f._sDownloadUrl).ToList();
                            if (downloadUrls.Any())
                            {
                                var progressWindow = new ProgressWindow($"Downloading {selectedMod._sName}", Application.Current.MainWindow);
                                progressWindow.Show();
                                foreach (var url in downloadUrls)
                                {
                                    await PrepareToDownloadFile();
                                    await DownloadFileAsync(url, progressWindow);
                                }
                                progressWindow.Close();
                                var file = Directory.GetFiles(ModsLaunchHelper.TempModsFolderPath).FirstOrDefault();
                                await ModInstallation.InstallModFromFileAsync(file);
                                Directory.Delete(ModsLaunchHelper.TempModsFolderPath, true);
                                MessageBox.Show("Mod downloaded and installed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("No downloadable files found for this mod.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to retrieve mod details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred during download: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a mod to download.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task PrepareToDownloadFile()
        {
            var tempFolder = ModsLaunchHelper.TempModsFolderPath;
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }

        private async Task DownloadFileAsync(string url, ProgressWindow progressWindow)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true }))
            {
                try
                {
                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    // Ensure we are following the final redirect to get the actual file.
                    var finalUrl = response.RequestMessage.RequestUri.ToString();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    // Check if the content disposition is available for the file name
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

        private string GetFileNameFromUrl(string url)
        {
            return Path.GetFileName(new Uri(url).AbsolutePath);
        }
    }
}
