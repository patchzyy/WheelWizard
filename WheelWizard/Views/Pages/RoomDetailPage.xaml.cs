using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;
using CT_MKWII_WPF.Classes;
using CT_MKWII_WPF.Utils;
using static CT_MKWII_WPF.Views.ViewUtils;

namespace CT_MKWII_WPF.Views.Pages
{
    public partial class RoomDetailPage : Page, INotifyPropertyChanged
    {
        private RoomInfo _room;
        public RoomInfo Room
        {
            get => _room;
            set
            {
                _room = value;
                OnPropertyChanged(nameof(Room));
            }
        }

        public ObservableCollection<KeyValuePair<string, RoomInfo.Player>> PlayersList { get; set; }
        

        public event PropertyChangedEventHandler PropertyChanged;

        public RoomDetailPage(RoomInfo room)
        {
            InitializeComponent();
            Room = room;
            PlayersList = new ObservableCollection<KeyValuePair<string, RoomInfo.Player>>(Room.Players);
            DataContext = this;
            LoadMiiImagesAsync();
            RoomKind.Text = HumanizeGameMode(room.Rk);
            PlayersListView.SortingFunctions.Add("Value.Ev", EvComparable);
            TimeOnline.Text = HumanizeTimeSpan(DateTime.UtcNow - room.Created);
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
        
        private static int EvComparable(object? x, object? y)
        {
            if (x is not KeyValuePair<string, RoomInfo.Player> xItem || 
                y is not KeyValuePair<string, RoomInfo.Player> yItem) return 0;
            if (!(int.TryParse(xItem.Value.Ev, out var xEv) &&
                  int.TryParse(yItem.Value.Ev, out var yEv))) return 0;
            return xEv.CompareTo(yEv);
        }
        
        private async void setMiiImage(KeyValuePair<String, RoomInfo.Player> playerPair)
        {
            var player = playerPair.Value;
            if (player.Mii == null || player.Mii.Count == 0 || string.IsNullOrEmpty(player.Mii[0]?.Data))
            {
                return;
            }
            if (player.Mii.Count > 0)
            {
                try
                {
                    var miiImage = await miiLoader.GetMiiImageAsync(player.Mii[0].Data);
                    player.MiiImage = miiImage;
                    Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(PlayersList)) );
                }
                catch (Exception ex)
                {
                    player.MiiImage = null;
                }
            }
        }

        private void LoadMiiImagesAsync()
        {
            foreach (var playerPair in PlayersList) 
                setMiiImage(playerPair);
        }
        

        private void GoBackClick(object sender, RoutedEventArgs e) => NavigateToPage(new RoomsPage());

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CopyFriendCode_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO: This list should not be a dictionary, it also makes this method a bit of a mess, since now i have to get it by getting an object and cast it to a KeyValuePair later
            var selectedMod = PlayersListView.GetCurrentContextItem<object>();

            if (selectedMod is KeyValuePair<string, RoomInfo.Player> playerPair)
            {
                IDataObject dataObject = new DataObject();
                dataObject.SetData(DataFormats.Text, playerPair.Value.Fc);
                Clipboard.SetDataObject(dataObject);
            }
        }
    }
    
}
