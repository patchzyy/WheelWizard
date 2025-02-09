using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WheelWizard.Models.GameData;
using WheelWizard.Services.WiiManagement.SaveData;
using WheelWizard.Utilities.RepeatedTasks;

namespace WheelWizard.Views.Pages;

public partial class FriendsPage : UserControl, INotifyPropertyChanged, IRepeatedTaskListener
{
    private ObservableCollection<GameDataFriend> _friendlist = new();
    public ObservableCollection<GameDataFriend> FriendList
    {
        get => _friendlist;
        set
        {   
            _friendlist = value;
            OnPropertyChanged(nameof(FriendList));
        }
    }
    public FriendsPage()
    {
        InitializeComponent();
        
        var data = GameDataLoader.Instance;
        data.Subscribe(this);
        UpdateFriendList(data);
        data.LoadGameData();
        
        DataContext = this;
        FriendsListView.ItemsSource = FriendList; 
        HandleVisibility();
    }
    public void OnUpdate(RepeatedTaskManager sender)
    {
        if (sender is not GameDataLoader gameDataLoader) return;
        UpdateFriendList(gameDataLoader);
    }
    
    private void UpdateFriendList(GameDataLoader gameDataLoader)
    { 
        var newList = gameDataLoader.GetCurrentFriends.OrderByDescending(f => f.IsOnline).ToList();
        for (var i = 0; i < newList.Count; i++) // Update existing items and add new ones
        {   
            if (i < FriendList.Count)
                FriendList[i] = newList[i];
            else
                FriendList.Add(newList[i]);
        }
        
        while (FriendList.Count > newList.Count)
        {
            FriendList.RemoveAt(FriendList.Count - 1);
        }
        ListItemCount.Text = FriendList.Count.ToString();
        HandleVisibility();
    }
    
    private void HandleVisibility()
    {
        VisibleWhenNoFriends.IsVisible = FriendList.Count <= 0;
        VisibleWhenFriends.IsVisible = FriendList.Count > 0;
    }
    
    private void SortByDropdown_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        
    }
    
    private enum ListOrderCondition
    {
        IS_ONLINE,
        VR,
        BR,
        NAME,
        WINS,
        TOTAL_RACES,
    }
        
    #region PropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
