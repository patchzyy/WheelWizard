﻿using CT_MKWII_WPF.Models.GameData;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using CT_MKWII_WPF.Utilities.RepeatedTasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Pages;

public partial class FriendsPage : Page, INotifyPropertyChanged, IRepeatedTaskListener
{
    
    private ObservableCollection<Friend> _friendlist = new();
    public ObservableCollection<Friend> FriendList
    {
        get => _friendlist;
        set
        {   
            _friendlist = value;
            OnPropertyChanged(nameof(FriendList));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnUpdate(RepeatedTaskManager sender)
    {
        if (sender is not GameDataLoader gameDataLoader) return;
        UpdateFriendList(gameDataLoader);
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
        LoadFirstFriend();
        HandleVisibility();
    }
    
    private void UpdateFriendList(GameDataLoader gameDataLoader)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var newList = gameDataLoader.GetCurrentFriends.OrderByDescending(f => f.IsOnline).ToList();
            
            // Update existing items and add new ones
            for (int i = 0; i < newList.Count; i++)
            {
                if (i < FriendList.Count)
                    FriendList[i] = newList[i];
                else
                    FriendList.Add(newList[i]);
            }

            // Remove extra items
            while (FriendList.Count > newList.Count)
            {
                FriendList.RemoveAt(FriendList.Count - 1);
            }

            HandleVisibility();
        });
    }
    private void HandleVisibility()
    {
        if (FriendList.Count == 0)
        {
            NoFriendsLabel.Visibility = Visibility.Visible;
            PlayerStats.Visibility = Visibility.Collapsed;
            FriendsListView.Visibility = Visibility.Collapsed;
        }
        else
        {
            NoFriendsLabel.Visibility = Visibility.Collapsed;
            PlayerStats.Visibility = Visibility.Visible;
            FriendsListView.Visibility = Visibility.Visible;
        }
    }

    private void LoadFirstFriend()
    {
        var firstFriend = FriendList.FirstOrDefault();
        if (firstFriend == null) return;
        PlayerStats.UpdateStats(firstFriend);
    }
    
    private static int VrComparable(object x, object y)
    {
        if (x is not Friend xItem || y is not Friend yItem) return 0;
        return xItem.Vr.CompareTo(yItem.Vr);
    }
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CopyFriendCode_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedFriend = FriendsListView.GetCurrentContextItem<Friend>();
        if (selectedFriend == null) return;
        Clipboard.SetText(selectedFriend.FriendCode);
    }


    private void FriendsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var selectedFriend = FriendsListView.SelectedItem as Friend;
        if (selectedFriend == null) return;
        
    }

    private void FriendListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedFriend = FriendsListView.SelectedItem as Friend;
        if (selectedFriend == null) return;
        PlayerStats.UpdateStats(selectedFriend);
    }
}

