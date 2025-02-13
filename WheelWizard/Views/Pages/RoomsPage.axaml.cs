using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WheelWizard.Models.RRInfo;
using WheelWizard.Services.LiveData;
using WheelWizard.Utilities.RepeatedTasks;

namespace WheelWizard.Views.Pages;

public partial class RoomsPage : UserControl, INotifyPropertyChanged, IRepeatedTaskListener
{
    private readonly ObservableCollection<RrRoom> _rooms = new();
    public ObservableCollection<RrRoom> Rooms
    {
        get => _rooms;
        init
        {
            _rooms = value;
            OnPropertyChanged(nameof(Rooms));
        }
    }

    private readonly ObservableCollection<RrPlayer> _players = new();
    public ObservableCollection<RrPlayer> Players
    {
        get => _players;
        init
        {
            _players = value;
            OnPropertyChanged(nameof(Players));
        }
    }

    public RoomsPage()
    {
        InitializeComponent();
        DataContext = this;
        
        Rooms = new ObservableCollection<RrRoom>(RRLiveRooms.Instance.CurrentRooms);
        RRLiveRooms.Instance.Subscribe(this);
        
        RoomsView.ItemsSource = Rooms;
        PlayersListView.ItemsSource = Players;
        
        OnUpdate(RRLiveRooms.Instance);
        Unloaded += RoomsPage_Unloaded;
    }
    
    public void OnUpdate(RepeatedTaskManager sender)
    {
        if (sender is not RRLiveRooms liveRooms) return;

        Rooms.Clear();
        var count = liveRooms.RoomCount;
        EmptyRoomsView.IsVisible = count == 0;
        RoomsView.IsVisible = count != 0;
        if (count == 0) return;

        foreach (var room in liveRooms.CurrentRooms)
            Rooms.Add(room);
        
        PerformSearch(null);
    }
    
    private void PerformSearch(string? query)
    {
        var isStringEmpty = string.IsNullOrWhiteSpace(query);
        RoomsView.IsVisible = isStringEmpty;
        PlayersListView.IsVisible = !isStringEmpty;
        
        if (isStringEmpty) return;

        Players.Clear();
        var matchingPlayers = Rooms
            .SelectMany(r => r.Players.Values)
            .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        p.Fc.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        foreach (var player in matchingPlayers)
        {
            Players.Add(player);
        }
    }
    
    private void RoomsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        RRLiveRooms.Instance.Unsubscribe(this);
    }

    private void CopyFriendCode_OnClick(object sender, RoutedEventArgs e)
    {
        // var selectedPlayer = PlayersListView.GetCurrentContextItem<RrPlayer>();
        // if (selectedPlayer == null) return;
        // TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(selectedPlayer.Fc);
    }
    
    private void PlayerSearchField_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.Source is not TextBox textBox) return;
        PerformSearch(textBox.Text);
    }
    
    #region PropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
