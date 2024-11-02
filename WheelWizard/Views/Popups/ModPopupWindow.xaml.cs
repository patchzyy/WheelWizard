using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Services.GameBanana;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using System.IO;
using CT_MKWII_WPF.Views.Components;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Popups
{
    public partial class ModPopupWindow : PopupContent, INotifyPropertyChanged
    {
        // Collection to hold the mods
        private ObservableCollection<ModRecord> Mods { get; set; } = new ObservableCollection<ModRecord>();

        // Pagination variables
        private int _currentPage = 1;
        private bool _isLoading = false;
        private bool _hasMoreMods = true;
        private bool _isInitialLoad = true;

        private const int ModsPerPage = 15;
        private const double ScrollThreshold = 50; // Adjusted threshold for earlier loading

        private string CurrentSearchTerm = "";
        private ScrollViewer _listViewScrollViewer;

        public ModPopupWindow() : base(true, false, false, "Mod Browser", new Vector(800, 800))
        {
            InitializeComponent();
            DataContext = this;
            ModListView.ItemsSource = Mods;
            
            // Initially, no mod is selected
            ModDetailViewer.Visibility = Visibility.Collapsed;
            EmptyDetailsView.Visibility = Visibility.Visible;

            ModDetailViewer.DownloadRequested += ModDetailViewer_DownloadRequested;
            
            // Attach to Loaded event
            this.Loaded += ModPopupWindow_Loaded;
        }


        /// <summary>
        /// Finds the ScrollViewer within the ListView.
        /// </summary>
        private void ModPopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isInitialLoad)
            {
                LoadMods(_currentPage);
                _isInitialLoad = false;
            }
        }
        
        private void ModListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_listViewScrollViewer == null)
            {
                _listViewScrollViewer = FindScrollViewer(ModListView);
                if (_listViewScrollViewer != null)
                {
                    _listViewScrollViewer.ScrollChanged += ModListView_ScrollChanged;
                }
            }
        }

        /// <summary>
        /// Recursively searches for a ScrollViewer within a DependencyObject.
        /// </summary>
        private ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if (d == null) return null;
            
            if (d is ScrollViewer scrollViewer)
                return scrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                var result = FindScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Loads mods for the specified page and search term.
        /// </summary>
        private async Task LoadMods(int page, string searchTerm = "")
        {
            if (_isLoading || !_hasMoreMods)
                return;

            _isLoading = true;
            try
            {
                var result = await GamebananaSearchHandler.SearchModsAsync(searchTerm, page, ModsPerPage);

                if (result.Succeeded && result.Content != null)
                {
                    var newMods = result.Content._aRecords
                        .Where(mod => mod._sModelName == "Mod")
                        .ToList();

                    if (newMods.Count > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            foreach (var mod in newMods)
                            {
                                Mods.Add(mod);
                            }
                        });

                        _hasMoreMods = newMods.Count >= ModsPerPage;
                        _currentPage = page;
                    }
                    else
                    {
                        _hasMoreMods = false;
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Failed to load mods: " + result.StatusMessage,
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    });
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("An error occurred: " + ex.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        /// <summary>
        /// Handles the ScrollChanged event to implement infinite scrolling.
        /// </summary>
        private async void ModListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_listViewScrollViewer == null || _isLoading || !_hasMoreMods)
                return;

            // Calculate remaining scroll distance
            var remainingScroll = _listViewScrollViewer.ScrollableHeight - _listViewScrollViewer.VerticalOffset;

            // Load more when we're within the threshold of the bottom
            if (remainingScroll <= ScrollThreshold)
            {
                await LoadMods(_currentPage + 1, CurrentSearchTerm);
            }
        }

        /// <summary>
        /// Handles the Search button click event.
        /// </summary>
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            CurrentSearchTerm = SearchTextBox.Text?.Trim() ?? "";
            _currentPage = 1;
            _hasMoreMods = true;
            
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mods.Clear();
            });
            
            await LoadMods(_currentPage, CurrentSearchTerm);
        }

        /// <summary>
        /// Handles the selection change in the ListView to display mod details.
        /// </summary>
        private async void ModListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModListView.SelectedItem is ModRecord selectedMod)
            {
                // Hide the empty view and show the detail viewer
                EmptyDetailsView.Visibility = Visibility.Collapsed;
                ModDetailViewer.Visibility = Visibility.Visible;

                // Load the selected mod details into the ModDetailViewer
                await ModDetailViewer.LoadModDetailsAsync(selectedMod);
            }
            else
            {
                // Show the empty view and hide the detail viewer
                ModDetailViewer.ClearDetails();
                ModDetailViewer.Visibility = Visibility.Collapsed;
                EmptyDetailsView.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Public method to load mod details independently from the list.
        /// </summary>
        public async Task LoadModDetailsExternallyAsync(ModRecord mod)
        {
            if (mod == null)
                return;

            // Hide the list view and show the detail viewer
            ModListView.SelectedItem = null;
            EmptyDetailsView.Visibility = Visibility.Collapsed;
            ModDetailViewer.Visibility = Visibility.Visible;

            // Load the mod details
            await ModDetailViewer.LoadModDetailsAsync(mod);
        }

        /// <summary>
        /// Event handler for download requests from the ModDetailViewer.
        /// </summary>
        private async void ModDetailViewer_DownloadRequested(object sender, DownloadRequestedEventArgs e)
        {
            await DownloadModAsync(e.Mod);
        }

        /// <summary>
        /// Handles the mod download and installation process.
        /// </summary>
        public async Task DownloadModAsync(ModRecord mod)
        {
            if (mod == null)
            {
                MessageBox.Show("No mod selected to download.", 
                                "Information", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Information);
                return;
            }

            var confirmation = MessageBox.Show($"Do you want to download and install the mod: {mod._sName}?",
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
                var modDetailResult = await GamebananaSearchHandler.GetModDetailsAsync(mod._idRow);
                if (modDetailResult.Succeeded && modDetailResult.Content != null)
                {
                    var downloadUrls = modDetailResult.Content._aFiles
                                       .Select(f => f._sDownloadUrl)
                                       .ToList();
                    if (downloadUrls.Any())
                    {
                        var progressWindow = new ProgressWindow($"Downloading {mod._sName}", Application.Current.MainWindow);
                        progressWindow.Show();
                        foreach (var url in downloadUrls)
                        {
                            await PrepareToDownloadFile();
                            await DownloadFileAsync(url, progressWindow);
                        }
                        progressWindow.Close();
                        var file = Directory.GetFiles(ModsLaunchHelper.TempModsFolderPath).FirstOrDefault();
                        if (file != null)
                        {
                            await ModInstallation.InstallModFromFileAsync(file);
                            Directory.Delete(ModsLaunchHelper.TempModsFolderPath, true);
                            MessageBox.Show("Mod downloaded and installed successfully!", 
                                            "Success", 
                                            MessageBoxButton.OK, 
                                            MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Downloaded file not found.", 
                                            "Error", 
                                            MessageBoxButton.OK, 
                                            MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No downloadable files found for this mod.", 
                                        "Info", 
                                        MessageBoxButton.OK, 
                                        MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve mod details.", 
                                    "Error", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Error);
                }
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

        // Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
