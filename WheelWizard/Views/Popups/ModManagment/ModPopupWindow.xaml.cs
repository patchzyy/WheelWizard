using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Services.GameBanana;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Popups;

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

    private string _currentSearchTerm = "";
    private ScrollViewer _listViewScrollViewer;

    public ModPopupWindow() : base(true, false, false, "Mod Browser", new Vector(800, 800))
    {
        InitializeComponent();
        DataContext = this;
        ModListView.ItemsSource = Mods;
        
        ModDetailViewer.Visibility = Visibility.Collapsed;
        EmptyDetailsView.Visibility = Visibility.Visible;
        Loaded += ModPopupWindow_Loaded;
    }

    /// <summary>
    /// Finds the ScrollViewer within the ListView.
    /// </summary>
    private void ModPopupWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_isInitialLoad) return;
        LoadMods(_currentPage).ConfigureAwait(false);
        _isInitialLoad = false;
    }

    private void ModListView_Loaded(object sender, RoutedEventArgs e)
    {
        if (_listViewScrollViewer != null) return;
        _listViewScrollViewer = FindScrollViewer(ModListView);
        _listViewScrollViewer.ScrollChanged += ModListView_ScrollChanged;
        
    }

    /// <summary>
    /// Recursively searches for a ScrollViewer within a DependencyObject.
    /// </summary>
    private ScrollViewer? FindScrollViewer(DependencyObject d)
    {
        if (d is ScrollViewer scrollViewer)
        {
            return scrollViewer;
        }
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
        {
            var child = VisualTreeHelper.GetChild(d, i);
            var result = FindScrollViewer(child);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// Loads mods for the specified page and search term.
    /// </summary>
    private async Task LoadMods(int page, string searchTerm = "")
    {
        if (_isLoading) return;
        if (!_hasMoreMods) return;
        _isLoading = true;
        try
        {
            var result = await GamebananaSearchHandler.SearchModsAsync(searchTerm, page, ModsPerPage);

            if (result is { Succeeded: true, Content: not null })
            {
                var metadata = result.Content._aMetadata;
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
                    _hasMoreMods = !metadata._bIsComplete;
                    _currentPage = page;
                }
                else
                {
                    // If no new mods were fetched, rely on metadata
                    _hasMoreMods = !result.Content._aMetadata._bIsComplete;
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new YesNoWindow().SetMainText("Failed to load mods")
                        .SetExtraText("An error occurred while loading mods.");
                });
            }
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new YesNoWindow().SetMainText("Failed to load mods")
                    .SetExtraText("An error occurred while loading mods." + ex.Message);
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
        if (_isLoading) return;
        if (!_hasMoreMods) return;
        

        // Calculate remaining scroll distance
        var remainingScroll = _listViewScrollViewer.ScrollableHeight - _listViewScrollViewer.VerticalOffset;

        // Load more when we're within the threshold of the bottom
        if (remainingScroll <= ScrollThreshold)
        {
            await LoadMods(_currentPage + 1, _currentSearchTerm);
        }
    }

    /// <summary>
    /// Handles the Search button click event.
    /// </summary>
    private async void Search_Click(object sender, RoutedEventArgs e)
    {
        _currentSearchTerm = SearchTextBox.Text.Trim();
        _currentPage = 1;
        _hasMoreMods = true;

        Application.Current.Dispatcher.Invoke(() =>
        {
            Mods.Clear();
        });

        await LoadMods(_currentPage, _currentSearchTerm);
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
            await ModDetailViewer.LoadModDetailsAsync(selectedMod._idRow);
        }
        else
        {
            // Show the empty view and hide the detail viewer
            ModDetailViewer.HideViewer();
            EmptyDetailsView.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Public method to load mod details independently of the list.
    /// </summary>
    public async Task LoadModDetailsExternallyAsync(ModRecord mod)
    {

        // Hide the list view and show the detail viewer
        ModListView.SelectedItem = null;
        EmptyDetailsView.Visibility = Visibility.Collapsed;
        ModDetailViewer.Visibility = Visibility.Visible;

        // Load the mod details
        await ModDetailViewer.LoadModDetailsAsync(mod._idRow);
    }

    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

