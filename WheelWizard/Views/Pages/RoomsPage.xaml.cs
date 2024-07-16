using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CT_MKWII_WPF.Classes;
using CT_MKWII_WPF.Utils;

namespace CT_MKWII_WPF.Views.Pages
{
    public partial class RoomsPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<RoomViewModel> _rooms;
        public ObservableCollection<RoomViewModel> Rooms
        {
            get => _rooms;
            set
            {
                _rooms = value;
                OnPropertyChanged(nameof(Rooms));
            }
        }

        public RoomsPage()
        {
            InitializeComponent();
            DataContext = this;
            Rooms = new ObservableCollection<RoomViewModel>();
            LoadRoomsData();
          
            RoomsView.SortingFunctions.Add("Players", PlayerComparable);
        }

        private static int PlayerComparable(object? x, object? y)
        {
            if (x is not RoomViewModel xItem || y is not RoomViewModel yItem) return 0;
            if (!(int.TryParse(xItem.PlayerCount, out var xPlayerCount) &&
                  int.TryParse(yItem.PlayerCount, out var yPlayerCount))) return 0;
            return xPlayerCount.CompareTo(yPlayerCount);
        }
        
        private async void LoadRoomsData()
        {
            try
            {
                var rrInfo = await RRLiveInfo.getCurrentGameData();
                UpdateRoomsList(rrInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rooms data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRoomsList(RRLiveInfo.RRInformation rrInfo)
        {
            Rooms.Clear();
            // TODO: Playtime looks cool, but the sorting is broken since its a string, not a real date anymore
            //      that means if you want to shortest time on top, 1 hour would still be higher then 30 minutes
            //      we can make a custom sorter function for this, but that is just moving the problem, so instead this model should probably contain a real DateTime object
            //      (or like a number of seconds since creation if date is not possible)
            foreach (var room in rrInfo.Rooms)
            {
                Rooms.Add(new RoomViewModel
                {
                    RoomInfo = room,
                    RoomNumber = room.Id,
                    PlayerCount = room.Players.Count.ToString(),
                    Type = room.Type,
                    RoomKind = HumanizeGameMode(room.Rk),
                    Playtime = HumanizeTimeSpan(DateTime.UtcNow - room.Created)
                });
            }

            EmptyRoomsView.Visibility = Rooms.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            RoomsView.Visibility = Rooms.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        
        private string HumanizeGameMode(string mode)
        {
            return mode switch
            {
                "vs_10" => "VS",
                "vs_11" => "TT",
                "vs_751" => "VS",
                _ => "??"
            };
        }
        

        
        private string HumanizeTimeSpan(TimeSpan timeSpan)
        {
            string P(int count) => count != 1 ? "s" : "";

            if (timeSpan.TotalDays >= 1)
                return $"{timeSpan.Days} day{P(timeSpan.Days)} {timeSpan.Hours} hour{P(timeSpan.Hours)}";
           
            if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.Hours} hour{P(timeSpan.Hours)} {timeSpan.Minutes} minute{P(timeSpan.Minutes)}";
        
            if (timeSpan.TotalMinutes >= 1)
                return $"{timeSpan.Minutes} minute{P(timeSpan.Minutes)} {timeSpan.Seconds} second{P(timeSpan.Seconds)}";
            
            return $"{timeSpan.Seconds} second{P(timeSpan.Seconds)}";
        } 

        private void Room_MouseDoubleClick(object sender, MouseButtonEventArgs e, ListViewItem clickedItem)
        {
            var selectedRoom = (RoomViewModel)clickedItem.DataContext;
            var roomDetailPage = new RoomDetailPage(selectedRoom.RoomInfo);
            NavigationService?.Navigate(roomDetailPage);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }  
    }

    public class RoomViewModel
    {
        public RoomInfo RoomInfo { get; set; }
        public string RoomNumber { get; set; }
        public string PlayerCount { get; set; }
        public string Type { get; set; }
        public string Playtime { get; set; }
        
        public string RoomKind { get; set; }
    }
}
