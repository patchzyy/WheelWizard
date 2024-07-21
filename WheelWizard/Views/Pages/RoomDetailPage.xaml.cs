using System;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.Other;
using CT_MKWII_WPF.Services.RetroRewind;
using CT_MKWII_WPF.Services.WiiManagement;
using CT_MKWII_WPF.Utilities.RepeatedTasks;
using static CT_MKWII_WPF.Views.ViewUtils;

namespace CT_MKWII_WPF.Views.Pages;

public partial class RoomDetailPage : Page, INotifyPropertyChanged, IRepeatedTaskListener
{
    private Room _room;
    public Room Room 
    {
        get => _room;
        set
        {
            _room = value;
            OnPropertyChanged(nameof(Room));
        }
    }
    
    private readonly ObservableCollection<Player> _playersList = new();
    public ObservableCollection<Player> PlayersList 
    {
        get => _playersList;
        init
        {
            _playersList = value;
            OnPropertyChanged(nameof(PlayersList));
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public RoomDetailPage(Room room)
    {
        InitializeComponent();
        Room = room;
        PlayersList = new ObservableCollection<Player>(Room.Players.Values);
        DataContext = this;
 
        RRLiveRooms.Instance.Subscribe(this);
        PlayersListView.SortingFunctions.Add("Ev", VrComparable);
        
        this.Unloaded += RoomsDetailPage_Unloaded;
    }
    
    private static int VrComparable(object? x, object? y)
    {
        if (x is not Player xItem || y is not Player yItem) return 0;
        return xItem.Vr.CompareTo(yItem.Vr);
    }

    public void OnUpdate(RepeatedTaskManager sender)
    {
        if (sender is not RRLiveRooms liveRooms) return;
        var room = liveRooms.CurrentRooms.Find(r => r.Id == Room.Id);
        if (room == null) return;
        // TODO: if the room is not found we can show that in the page something like "this room has been closed"
        
        Room = room;
        PlayersList.Clear();
        foreach (var p in room.Players.Values)
            PlayersList.Add(p);
    }
    
    private void GoBackClick(object sender, RoutedEventArgs e) => NavigateToPage(new RoomsPage());

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CopyFriendCode_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedPlayer = PlayersListView.GetCurrentContextItem<Player>();
        if (selectedPlayer == null) return;
        IDataObject dataObject = new DataObject();
        dataObject.SetData(DataFormats.Text, selectedPlayer.Fc);
        Clipboard.SetDataObject(dataObject);
    }
    
    private void RoomsDetailPage_Unloaded(object sender, RoutedEventArgs e)
    {
        RRLiveRooms.Instance.Unsubscribe(this);
    }
}