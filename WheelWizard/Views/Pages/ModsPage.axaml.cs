using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WheelWizard.Models.Settings;
using WheelWizard.Services;
using WheelWizard.Views.Popups.Generic;
using WheelWizard.Views.Popups.ModManagement;
using ModPopupWindow = WheelWizard.Views.Popups.ModManagement.ModPopupWindow;

namespace WheelWizard.Views.Pages;

public partial class ModsPage : UserControl, INotifyPropertyChanged
{
    public ModManager ModManager => ModManager.Instance;
    public ObservableCollection<Mod> Mods => ModManager.Mods;
    
    private int _draggedIndex = -1;
    private Point _startPoint;

    private bool _hasMods;
    public bool HasMods
    {
        get => _hasMods;
        set
        {
            if (_hasMods == value) return;

            _hasMods = value;
            OnPropertyChanged();
            UpdateVisibility();
        }
    }

     private void UpdateVisibility()
     {
        Dispatcher.UIThread.InvokeAsync(() => {
             PageWithoutMods.IsVisible = !HasMods;
            TopBarButtons.IsVisible = HasMods;
            PageWithMods.IsVisible = HasMods;
        });
     }

    public ModsPage()
    {
        InitializeComponent();
        DataContext = this;
        SubscribeToModManagerEvents();
        ModManager.ReloadAsync();
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

    private void OnModsLoaded() => UpdateEmptyListMessageVisibility();
    private void OnModsChanged() => UpdateEmptyListMessageVisibility();
    private void UpdateEmptyListMessageVisibility() => HasMods = Mods.Count > 0;
     
    private void OnModProcessingStarted() => ShowLoading(true);
    private void OnModProcessingCompleted() => ShowLoading(false);
    
    private void OnModProcessingProgress(int current, int total, string status)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            ProgressBar.Value = (double)current / total * 100;
            StatusTextBlock.Text = status;
        }, DispatcherPriority.Background);
    }

    private void OnErrorOccurred(string message)
    {
        new MessageBoxWindow()
            .SetMessageType(MessageBoxWindow.MessageType.Error)
            .SetTitleText("An error occured")
            .SetInfoText(message)
            .Show();
    }
    
    private void BrowseMod_Click(object sender, RoutedEventArgs e)
    {
        var modPopup = new ModPopupWindow();
        modPopup.Show();
    }
    private void ImportMod_Click(object sender, RoutedEventArgs e)
    {
        ModManager.ImportMods();
    }
    
    private void ModsListBox_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Source is not ListBox source)
              return;
         
        if (!e.Data.Contains(DataFormats.Text))
            e.DragEffects = DragDropEffects.None;
    }
      
    private void ModsListBox_DragOver(object sender, DragEventArgs e)
    {
        if (e.Source is not ListBox source)
            return;
        
        if (!e.Data.Contains(DataFormats.Text))
            e.DragEffects = DragDropEffects.None;
    }

    private void ModsListBox_Drop(object sender, DragEventArgs e)
    {
        if (e.Source is not ListBox source)
              return;
        
        var targetIndex = FindIndex(source, e.GetPosition(source).Y);
        if (_draggedIndex == -1 || targetIndex == -1 || targetIndex == _draggedIndex) return;

        ModManager.ReorderMod(Mods[_draggedIndex],targetIndex);
        _draggedIndex = -1;


    }
     
    private int FindIndex(ListBox listBox, double Y) {

        for (var i = 0; i < listBox.ItemCount; i++) {
            var item = listBox.ContainerFromIndex(i) as ListBoxItem;
            if (item != null && Y < (item.TranslatePoint(new Point(0,item.Bounds.Height), listBox).Value.Y)) {
                return i;
            }
        }
        return -1;
    }

    private void RenameMod_Click(object sender, RoutedEventArgs e)
    {
        if (ModsListBox.SelectedItem is not Mod selectedMod)
             return;
        ModManager.RenameMod(selectedMod);
        OnModsChanged();
    }

    private void DeleteMod_Click(object sender, RoutedEventArgs e)
    {
        if (ModsListBox.SelectedItem is not Mod selectedMod)
            return;
        
        ModManager.DeleteMod(selectedMod);
        OnModsChanged();
        ModManager.LoadModsAsync();
    }
    
    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if (ModsListBox.SelectedItem is not Mod selectedMod)
            return;
        
        ModManager.OpenModFolder(selectedMod);
    }
    
        private void ViewMod_Click(object sender, RoutedEventArgs e)
    {
        if (ModsListBox.SelectedItem is not Mod selectedMod)
        {
            // You actually never see this error, however, if for some unknown reason it happens, we don't want to disregard it
            new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Warning)
                .SetTitleText("Cannot view the selected mod")
                .SetInfoText("Something went wrong when trying to open the selected mod")
                .Show();
            return;
        }
       
        if (selectedMod.ModID == -1)
        {
            new MessageBoxWindow()
                .SetMessageType(MessageBoxWindow.MessageType.Warning)
                .SetTitleText("Cannot view the selected mod")
                .SetInfoText("Cannot view mod that was not installed through the mod browser.")
                .Show();
            return;
        }
        
        var modPopup = new ModIndependentWindow();
        modPopup.LoadModAsync(selectedMod.ModID);
        modPopup.ShowDialog();
    }


    private void ShowLoading(bool isLoading)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            ProgressBar.IsVisible = isLoading;
            StatusTextBlock.IsVisible = isLoading;
        });
    }
    
    #region PropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
