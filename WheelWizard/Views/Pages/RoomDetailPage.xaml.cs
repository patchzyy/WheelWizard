using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;
using CT_MKWII_WPF.Utils;

namespace CT_MKWII_WPF.Views.Pages
{
    public partial class RoomDetailPage : Page
    {
        public RRLiveInfo.RoomInfo Room { get; set; }
        private static readonly HttpClient _httpClient = new HttpClient();

        public RoomDetailPage(RRLiveInfo.RoomInfo room)
        {
            InitializeComponent();
            Room = room;
            DataContext = this;

            playersListView.MouseDoubleClick += PlayersListView_MouseDoubleClick;
        }

        private void GoBackClick(object sender, RoutedEventArgs e)
        {
            var layout = (Layout)Application.Current.MainWindow;
            layout.NavigateToPage(new RoomsPage());
        }

        private async void PlayersListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (playersListView.SelectedItem is KeyValuePair<string, RRLiveInfo.RoomInfo.Player> selectedPlayer)
            {
                try
                {
                    if (selectedPlayer.Value.Mii != null && selectedPlayer.Value.Mii.Count > 0)
                    {
                        var miiImage = await GetMiiImageAsync(selectedPlayer.Value.Mii[0].Data);
                        ShowMiiPopup(selectedPlayer.Value.Name, miiImage);
                    }
                    else
                    {
                        MessageBox.Show("No Mii data available for this player.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Network error: {ex.Message}\n\nPlease check your internet connection and try again.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Error parsing Mii data: {ex.Message}\n\nThe server response was not in the expected format.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\nPlease try again later or contact support if the issue persists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task<BitmapImage> GetMiiImageAsync(string base64MiiData)
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new ByteArrayContent(Convert.FromBase64String(base64MiiData)), "data", "mii.dat");
            formData.Add(new StringContent("wii"), "platform");

            var response = await _httpClient.PostAsync("https://qrcode.rc24.xyz/cgi-bin/studio.cgi", formData);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);
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
            // Construct the URL using the Mii data from the response
            var baseUrl = "https://studio.mii.nintendo.com/miis/image.png";
            var queryParameters = $"data={response.MiiData}&type=face&expression=normal&width=270&bgColor=FFFFFF00&clothesColor=default&cameraXRotate=0&cameraYRotate=0&cameraZRotate=0&characterXRotate=0&characterYRotate=0&characterZRotate=0&lightDirectionMode=none&instanceCount=1&instanceRotationMode=model";
            
            return $"{baseUrl}?{queryParameters}";
        }

        private void ShowMiiPopup(string playerName, BitmapImage miiImage)
        {
            var window = new Window
            {
                Title = $"{playerName}'s Mii",
                Content = new Image
                {
                    Source = miiImage,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(10)
                },
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }
    }

    public class MiiResponse
    {
        [JsonPropertyName("mii")]
        public string MiiData { get; set; }

        [JsonPropertyName("miistudio")]
        public string MiiStudio { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("creator_name")]
        public string CreatorName { get; set; }

        [JsonPropertyName("birthday")]
        public string Birthday { get; set; }

        [JsonPropertyName("favorite_color")]
        public string FavoriteColor { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("build")]
        public int Build { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("mingle")]
        public string Mingle { get; set; }

        [JsonPropertyName("copying")]
        public string Copying { get; set; }
    }
}
