using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.UrlProtocol;
using CT_MKWII_WPF.Views.Popups;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CT_MKWII_WPF.Views.Pages;

public partial class ModsPage : Page, INotifyPropertyChanged
{
    public ModManager ModManager => ModManager.Instance;
    public ObservableCollection<Mod> Mods => ModManager.Mods;

    private bool _toggleAll = true;
    private Point _startPoint;

    public ModsPage()
    {
        InitializeComponent();
        DataContext = this;
        SubscribeToModManagerEvents();
        ModManager.InitializeAsync();
    }

    private void SubscribeToModManagerEvents()
    {
        ModManager.ModsLoaded += OnModsLoaded;
        ModManager.ModsChanged += OnModsChanged;
        ModManager.ModProcessingStarted += OnModProcessingStarted;
        ModManager.ModProcessingCompleted += OnModProcessingCompleted;
        ModManager.ModProcessingProgress += OnModProcessingProgress;
        ModManager.ErrorOccurred += OnErrorOccurred;
    }

    private void OnModsLoaded()
    {
        UpdateEmptyListMessageVisibility();
    }

    private void OnModsChanged()
    {
        UpdateEmptyListMessageVisibility();
    }

    private void OnModProcessingStarted()
    {
        ShowLoading(true);
    }

    private void OnModProcessingCompleted()
    {
        ShowLoading(false);
    }

    private void OnModProcessingProgress(int current, int total, string status)
    {
        Dispatcher.Invoke(() =>
        {
            ProgressBar.Value = (double)current / total * 100;
            StatusTextBlock.Text = status;
        }, DispatcherPriority.Background);
    }

    private void OnErrorOccurred(string message)
    {
        MessageBoxWindow.ShowDialog(message);
    }

    private void UpdateEmptyListMessageVisibility()
    {
        Dispatcher.Invoke(() =>
        {
            PageWithoutMods.Visibility = Mods.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            TopBarButtons.Visibility = Mods.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            PageWithMods.Visibility = Mods.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        });
    }

    private void BrowseMod_Click(object sender, RoutedEventArgs e)
    {
        var modPopup = new Views.Popups.ModPopupWindow();
        modPopup.Show();
    }

    private void ImportMod_Click(object sender, RoutedEventArgs e)
    {
        ModManager.ImportMods();
    }

    private void EnableClick(object sender, RoutedEventArgs e)
    {
        ModManager.ToggleAllMods(_toggleAll);
        _toggleAll = !_toggleAll;
        // EnableDisableButton.Text = !_toggleAll ? Common.Action_DisableAll : Common.Action_EnableAll;
    }

    private void RenameMod_Click(object sender, RoutedEventArgs e)
    {
        var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
        if (selectedMod == null) return;

        ModManager.RenameMod(selectedMod);
    }

    private void DeleteMod_Click(object sender, RoutedEventArgs e)
    {
        var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
        if (selectedMod == null) return;

        ModManager.DeleteMod(selectedMod);
    }

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        var selectedMod = ModsListView.GetCurrentContextItem<Mod>();
        if (selectedMod == null) return;

        ModManager.OpenModFolder(selectedMod);
    }

    private void ModsListView_OnOnItemsReorder(ListViewItem movedItem, int newIndex)
    {
        var movedMod = (Mod)movedItem.DataContext;
        ModManager.ReorderMod(movedMod, newIndex);
    }

    private void CheckIfSet(object sender, RoutedEventArgs e)
    {
        bool isRegistered = UrlProtocolManager.IsCustomSchemeRegistered(UrlProtocolManager.ProtocolName);
        MessageBoxWindow.ShowDialog(isRegistered ? "The URL scheme is registered" : "The URL scheme is not registered");
    }

    private void RemoveUrlScheme(object sender, RoutedEventArgs e)
    {
        UrlProtocolManager.RemoveCustomScheme(UrlProtocolManager.ProtocolName);
    }

    private void ShowLoading(bool isLoading)
    {
        Dispatcher.Invoke(() =>
        {
            ProgressBar.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            StatusTextBlock.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

