using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.Networking;

namespace CT_MKWII_WPF.Views.Pages;

public sealed partial class RoomsPage : Page, INotifyPropertyChanged
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

    public RoomsPage()
    {
        InitializeComponent();
        DataContext = this;
        EmptyRoomsView.Visibility = RRLiveRooms.RoomCount == 0 ? Visibility.Visible : Visibility.Collapsed;
        RoomsView.Visibility = RRLiveRooms.RoomCount == 0 ? Visibility.Collapsed : Visibility.Visible;
        Rooms = new ObservableCollection<Room>(RRLiveRooms.CurrentRooms);
        RRLiveRooms.SubscribeToRoomsUpdated(UpdateRoomsList);
        RoomsView.SortingFunctions.Add("Players", PlayerCountComparable);
        RoomsView.SortingFunctions.Add("TimeOnline", TimeOnlineComparable);
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

    private void UpdateRoomsList()
    {
        Console.WriteLine("THIS SUBSCRIBTION GETS ADDED BUT NEVER REMOVED, LEAVE THIS HERE UNTIL IT IS FIXED, KIND REGARDS, WANTTOBEEME");
        Rooms.Clear();
        var count = RRLiveRooms.RoomCount;
        EmptyRoomsView.Visibility = count == 0 ? Visibility.Visible : Visibility.Collapsed;
        RoomsView.Visibility = count == 0 ? Visibility.Collapsed : Visibility.Visible;
        if (count == 0) return;
        
        foreach (var room in RRLiveRooms.CurrentRooms)
        {
            Rooms.Add(room);
        }
    }
    

    private void Room_MouseDoubleClick(object sender, MouseButtonEventArgs e, ListViewItem clickedItem)
    {
        var selectedRoom = (Room)clickedItem.DataContext;
        var roomDetailPage = new RoomDetailPage(selectedRoom);
        NavigationService?.Navigate(roomDetailPage);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    //protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    //{
    //    Console.WriteLine("LEAVING TEST");
    //    RRLiveRooms.UnsubscribeToRoomsUpdated(UpdateRoomsList);
    //}
}