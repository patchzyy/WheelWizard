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
            if (_hasMods != value)
            {
                _hasMods = value;
                OnPropertyChanged();
                UpdateVisibility();
            }
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
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            ProgressBar.Value = (double)current / total * 100;
            StatusTextBlock.Text = status;
        }, DispatcherPriority.Background);
    }

    private void OnErrorOccurred(string message)
    {
        new MessageBoxWindow().SetMainText(message).Show();
    }


     private void UpdateEmptyListMessageVisibility()
    {
        HasMods = Mods.Count > 0;
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
          var source = e.Source as ListBox;
          if (source == null)
              return;
        if (!e.Data.Contains(DataFormats.Text))
        {
            e.DragEffects = DragDropEffects.None;
        }
         
    }
      
       private void ModsListBox_DragOver(object sender, DragEventArgs e)
    {
          var source = e.Source as ListBox;
          if (source == null)
              return;
        if (!e.Data.Contains(DataFormats.Text))
        {
            e.DragEffects = DragDropEffects.None;
        }
           
    }

    private void ModsListBox_Drop(object sender, DragEventArgs e)
    {
       
          var source = e.Source as ListBox;
          if (source == null)
              return;
          var targetIndex = FindIndex(source, e.GetPosition(source).Y);
        if (_draggedIndex != -1 && targetIndex != -1 && targetIndex != _draggedIndex)
        {
           ModManager.ReorderMod(Mods[_draggedIndex],targetIndex);
            _draggedIndex = -1;
        }
          
          
    }
     
        private int FindIndex(ListBox listBox, double Y) {

            for (int i = 0; i < listBox.ItemCount; i++) {
                var item = listBox.ContainerFromIndex(i) as ListBoxItem;
                if (item != null && Y < (item.TranslatePoint(new Point(0,item.Bounds.Height), listBox).Value.Y)) {
                    return i;
                }
            }
            return -1;
        }


    private void RenameMod_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.Parent is not ContextMenu contextMenu || contextMenu.Parent is not ListBox listBox)
            return;
        
        if (listBox.SelectedItem is not Mod selectedMod)
             return;
        ModManager.RenameMod(selectedMod);
        OnModsChanged();
    }

    private void DeleteMod_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.Parent is not ContextMenu contextMenu || contextMenu.Parent is not ListBox listBox)
            return;

        if (listBox.SelectedItem is not Mod selectedMod)
            return;
        ModManager.DeleteMod(selectedMod);
        OnModsChanged();
        ModManager.LoadModsAsync();
    }
    
    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.Parent is not ContextMenu contextMenu || contextMenu.Parent is not ListBox listBox)
            return;

        if (listBox.SelectedItem is not Mod selectedMod)
            return;
        ModManager.OpenModFolder(selectedMod);
    }
    
        private void ViewMod_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.Parent is not ContextMenu contextMenu || contextMenu.Parent is not ListBox listBox)
            return;
        
        if (listBox.SelectedItem is not Mod selectedMod)
            return;
        if (selectedMod == null || selectedMod.ModID == -1)
        {
            new MessageBoxWindow().SetMainText("Cannot view mod that was not installed through the mod browser").Show();
            return;
        }
        var ModID = selectedMod.ModID;
        //todo: put Modpopup avalinia window here
        // var modPopup = new ModIndependentWindow();
        // modPopup.LoadModAsync(ModID);
        // modPopup.ShowDialog();
        //temp just show message box
        new MessageBoxWindow().SetMainText("translate this to Avalonia!").Show();
        
    }


    private void ShowLoading(bool isLoading)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            ProgressBar.IsVisible = isLoading;
            StatusTextBlock.IsVisible = isLoading;
        });
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
