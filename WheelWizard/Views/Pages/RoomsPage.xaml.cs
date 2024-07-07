using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            
            // Add event handler for double-click
            RoomsListView.MouseDoubleClick += RoomsListView_MouseDoubleClick;
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
                MessageBox.Show($"Error loading rooms data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRoomsList(RRLiveInfo.RRInformation rrInfo)
        {
            Rooms.Clear();
            foreach (var room in rrInfo.Rooms)
            {
                Rooms.Add(new RoomViewModel
                {
                    RoomInfo = room,
                    RoomNumber = room.Id,
                    PlayerCount = room.Players.Count.ToString(),
                    Type = room.Type,
                    Playtime = (DateTime.Now - room.Created).ToString(@"hh\:mm\:ss")
                });
            }

            if (Rooms.Count == 0)
            {
                EmptyRoomsMessage.Visibility = Visibility.Visible;
                RoomsListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyRoomsMessage.Visibility = Visibility.Collapsed;
                RoomsListView.Visibility = Visibility.Visible;
            }
        }

        private void RoomsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRoom = RoomsListView.SelectedItem as RoomViewModel;
            if (selectedRoom != null)
            {
                var roomDetailPage = new RoomDetailPage(selectedRoom.RoomInfo);
                NavigationService?.Navigate(roomDetailPage);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void View_RoomButton(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    public class RoomViewModel
    {
        public RRLiveInfo.RoomInfo RoomInfo { get; set; }
        public string RoomNumber { get; set; }
        public string PlayerCount { get; set; }
        public string Type { get; set; }
        public string Playtime { get; set; }
    }
}