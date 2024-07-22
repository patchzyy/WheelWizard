using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.LiveData;
using CT_MKWII_WPF.Utilities.RepeatedTasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

    public RoomsPage()
    {
        InitializeComponent();
        DataContext = this;
        EmptyRoomsView.Visibility = RRLiveRooms.Instance.RoomCount == 0 ? Visibility.Visible : Visibility.Collapsed;
        RoomsView.Visibility = RRLiveRooms.Instance.RoomCount == 0 ? Visibility.Collapsed : Visibility.Visible;
        Rooms = new ObservableCollection<Room>(RRLiveRooms.Instance.CurrentRooms);
        RRLiveRooms.Instance.Subscribe(this);

        RoomsView.SortingFunctions.Add("Players", PlayerCountComparable);
        RoomsView.SortingFunctions.Add("TimeOnline", TimeOnlineComparable);

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
    }

    private void Room_MouseClick(object sender, MouseButtonEventArgs e, ListViewItem clickedItem)
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

    private void RoomsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        RRLiveRooms.Instance.Unsubscribe(this);
    }
}
