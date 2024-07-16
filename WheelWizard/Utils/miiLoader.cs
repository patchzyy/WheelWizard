using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Utils;

public class miiLoader
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    
    public static async Task<BitmapImage> GetMiiImageAsync(string base64MiiData)
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
    
    private static string GetMiiImageUrlFromResponse(MiiResponse response)
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