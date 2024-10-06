using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Services.LiveData;
using CT_MKWII_WPF.Utilities.RepeatedTasks;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace CT_MKWII_WPF.Views.Pages;

public sealed partial class RoomsPage : Page, INotifyPropertyChanged, IRepeatedTaskListener
{
    private readonly ObservableCollection<Room> _rooms = new();
    public ObservableCollection<Room> Rooms
    {
        get => _rooms;
        init
        {
            _rooms = value;
            OnPropertyChanged(nameof(Rooms));
        }
    }

    private readonly ObservableCollection<Player> _players = new();
    public ObservableCollection<Player> Players
    {
        get => _players;
        init
        {
            _players = value;
            OnPropertyChanged(nameof(Players));
        }
    }

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged(nameof(SearchQuery));
            PerformSearch();
        }
    }

    private Visibility _roomsListVisibility = Visibility.Visible;
    public Visibility RoomsListVisibility
    {
        get => _roomsListVisibility;
        set
        {
            _roomsListVisibility = value;
            OnPropertyChanged(nameof(RoomsListVisibility));
        }
    }

    private Visibility _playersListVisibility = Visibility.Collapsed;
    public Visibility PlayersListVisibility
    {
        get => _playersListVisibility;
        set
        {
            _playersListVisibility = value;
            OnPropertyChanged(nameof(PlayersListVisibility));
        }
    }

    public RoomsPage()
    {
        InitializeComponent();
        DataContext = this;
        EmptyRoomsView.Visibility = RRLiveRooms.Instance.RoomCount == 0 ? Visibility.Visible : Visibility.Collapsed;
        RoomsView.Visibility = RRLiveRooms.Instance.RoomCount == 0 ? Visibility.Collapsed : Visibility.Visible;
        Rooms = new ObservableCollection<Room>(RRLiveRooms.Instance.CurrentRooms);
        Players = new ObservableCollection<Player>();
        RRLiveRooms.Instance.Subscribe(this);

        RoomsView.SortingFunctions.Add(Online.Stat_PlayerCount, PlayerCountComparable);
        RoomsView.SortingFunctions.Add("TimeOnline", TimeOnlineComparable);

        PlayersListView.SortingFunctions.Add("Ev", VrComparable);

        this.Unloaded += RoomsPage_Unloaded;
    }

    private static int TimeOnlineComparable(object? x, object? y)
    {
        if (x is not Room xItem || y is not Room yItem) return 0;
        return xItem.Created.CompareTo(yItem.Created);
    }

    private static int PlayerCountComparable(object? x, object? y)
    {
        if (x is not Room xItem || y is not Room yItem) return 0;
        return xItem.PlayerCount.CompareTo(yItem.PlayerCount);
    }

    private static int VrComparable(object? x, object? y)
    {
        if (x is not Player xItem || y is not Player yItem) return 0;
        return xItem.Vr.CompareTo(yItem.Vr);
    }

    public void OnUpdate(RepeatedTaskManager sender)
    {
        if (sender is not RRLiveRooms liveRooms) return;

        Rooms.Clear();
        var count = liveRooms.RoomCount;
        EmptyRoomsView.Visibility = count == 0 ? Visibility.Visible : Visibility.Collapsed;
        RoomsView.Visibility = count == 0 ? Visibility.Collapsed : Visibility.Visible;
        if (count == 0) return;

        foreach (var room in liveRooms.CurrentRooms)
            Rooms.Add(room);

        PerformSearch(); // Update search results if there's an active search
    }

    private void Room_MouseClick(object sender, MouseButtonEventArgs e, ListViewItem clickedItem)
    {
        var selectedRoom = (Room)clickedItem.DataContext;
        var roomDetailPage = new RoomDetailPage(selectedRoom);
        NavigationService?.Navigate(roomDetailPage);
    }

    private void Player_MouseClick(object sender, MouseButtonEventArgs e, ListViewItem clickedItem)
    {
        var selectedPlayer = (Player)clickedItem.DataContext;
        var room = Rooms.FirstOrDefault(r => r.Players.ContainsValue(selectedPlayer));
        if (room != null)
        {
            var roomDetailPage = new RoomDetailPage(room);
            NavigationService?.Navigate(roomDetailPage);
        }
    }

    private void PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            RoomsView.Visibility = Visibility.Visible;
            PlayersListVisibility = Visibility.Collapsed;
            return;
        }

        Players.Clear();
        var matchingPlayers = Rooms
            .SelectMany(r => r.Players.Values)
            .Where(p => p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                        p.Fc.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        foreach (var player in matchingPlayers)
        {
            Players.Add(player);
        }

        RoomsView.Visibility = Visibility.Collapsed;
        PlayersListVisibility = Visibility.Visible;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RoomsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        RRLiveRooms.Instance.Unsubscribe(this);
    }

    private void CopyFriendCode_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedPlayer = PlayersListView.GetCurrentContextItem<Player>();
        if (selectedPlayer == null) return;
        IDataObject dataObject = new DataObject();
        dataObject.SetData(DataFormats.Text, selectedPlayer.Fc);
        Clipboard.SetDataObject(dataObject);
    }
}
