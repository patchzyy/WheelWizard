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
using CT_MKWII_WPF.Utils;
using System.ComponentModel;
using static CT_MKWII_WPF.Views.ViewUtils;

namespace CT_MKWII_WPF.Views.Pages
{
    public partial class RoomDetailPage : Page, INotifyPropertyChanged
    {
        private RRLiveInfo.RoomInfo _room;
        public RRLiveInfo.RoomInfo Room
        {
            get => _room;
            set
            {
                _room = value;
                OnPropertyChanged(nameof(Room));
            }
        }

        public ObservableCollection<KeyValuePair<string, RRLiveInfo.RoomInfo.Player>> PlayersList { get; set; }

        private static readonly HttpClient _httpClient = new HttpClient();

        public event PropertyChangedEventHandler PropertyChanged;

        public RoomDetailPage(RRLiveInfo.RoomInfo room)
        {
            InitializeComponent();
            Room = room;
            PlayersList = new ObservableCollection<KeyValuePair<string, RRLiveInfo.RoomInfo.Player>>(Room.Players);
            DataContext = this;

            PlayerCountBox.Text = Room.Players.Count.ToString();
            LoadMiiImagesAsync();
         
            PlayersListView.SortingFunctions.Add("Value.Ev", EvComparable);
        }
        
        private static int EvComparable(object? x, object? y)
        {
            if (x is not KeyValuePair<string, RRLiveInfo.RoomInfo.Player> xItem || 
                y is not KeyValuePair<string, RRLiveInfo.RoomInfo.Player> yItem) return 0;
            if (!(int.TryParse(xItem.Value.Ev, out var xEv) &&
                  int.TryParse(yItem.Value.Ev, out var yEv))) return 0;
            return xEv.CompareTo(yEv);
        }
        
        private async void setMiiImage(KeyValuePair<String, RRLiveInfo.RoomInfo.Player> playerPair)
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
                    var miiImage = await GetMiiImageAsync(player.Mii[0].Data);
                    player.MiiImage = miiImage;
                    Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(PlayersList)) );
                }
                catch (Exception ex)
                {
                    player.MiiImage = null;
                }
            }
        }

        private async void LoadMiiImagesAsync()
        {
            foreach (var playerPair in PlayersList) 
                setMiiImage(playerPair);
        }

        private async Task<BitmapImage> GetMiiImageAsync(string base64MiiData)
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new ByteArrayContent(Convert.FromBase64String(base64MiiData)), "data", "mii.dat");
            formData.Add(new StringContent("wii"), "platform");

            var response = await _httpClient.PostAsync("https://qrcode.rc24.xyz/cgi-bin/studio.cgi", formData);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error fetching Mii image: {response.StatusCode}");
            }

            var miiResponse = JsonSerializer.Deserialize<MiiResponse>(jsonResponse);

            var miiImageUrl = GetMiiImageUrlFromResponse(miiResponse);

            return new BitmapImage(new Uri(miiImageUrl));
        }

        private string GetMiiImageUrlFromResponse(MiiResponse response)
        {
            const string baseUrl = "https://studio.mii.nintendo.com/miis/image.png";
    
            var queryParams = new List<string>
            {
                $"data={response.MiiData}",
                "type=face", "expression=normal",
                "width=270",
                "bgColor=FFFFFF00",
                "clothesColor=default",
                "cameraXRotate=0", "cameraYRotate=0", "cameraZRotate=0",
                "characterXRotate=0", "characterYRotate=0", "characterZRotate=0",
                "lightDirectionMode=none",
                "instanceCount=1",
                "instanceRotationMode=model"
            };

            string queryString = string.Join("&", queryParams);
            return $"{baseUrl}?{queryString}";
        }

        private void GoBackClick(object sender, RoutedEventArgs e) => GetLayout().NavigateToPage(new RoomsPage());

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CopyFriendCode_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO: This list should not be a dictionary, it also makes this method a bit of a mess, since now i have to get it by getting an object and cast it to a KeyValuePair later
            var selectedMod = PlayersListView.GetCurrentContextItem<object>();
            if (selectedMod is KeyValuePair<string, RRLiveInfo.RoomInfo.Player> playerPair)
                Clipboard.SetText(playerPair.Value.Fc);
        }
    }

    public class MiiResponse
    {
        [JsonPropertyName("mii")] public string MiiData { get; set; }
        [JsonPropertyName("miistudio")] public string MiiStudio { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("creator_name")] public string CreatorName { get; set; }
        [JsonPropertyName("birthday")] public string Birthday { get; set; }
        [JsonPropertyName("favorite_color")] public string FavoriteColor { get; set; }
        [JsonPropertyName("height")] public int Height { get; set; }
        [JsonPropertyName("build")] public int Build { get; set; }
        [JsonPropertyName("gender")] public string Gender { get; set; }
        [JsonPropertyName("mingle")] public string Mingle { get; set; }
        [JsonPropertyName("copying")] public string Copying { get; set; }
    }
}
