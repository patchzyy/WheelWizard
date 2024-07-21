using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.RRInfo;
using Newtonsoft.Json;

namespace CT_MKWII_WPF.Services.WiiManagement;

public static class MiiImageManager
{
    private const int MaxCachedImages = 126;
    private static readonly Dictionary<string, BitmapImage> Images = new();
    private static readonly Queue<string> ImageOrder = new();
    
    public static BitmapImage? GetCachedMiiImage(string miiData) => Images.GetValueOrDefault(miiData);
    
    private static void AddMiiImage(string miiData, BitmapImage image)
    {
        if (!Images.ContainsKey(miiData))
            ImageOrder.Enqueue(miiData);
        // if it already exists, it will replace the old one. so the dict will not grow
        Images[miiData] = image;

        if (Images.Count < MaxCachedImages) return;
        var oldestMiiData = ImageOrder.Dequeue();
        Images.Remove(oldestMiiData);
    }

    public static async void LoadMiiImageAsync(Player player)
    {
        if (player.Mii.Count <= 0) return;
        
        var miiData = player.Mii[0].Data;
        var newImage = await GetMiiImageAsync(miiData);
        AddMiiImage(miiData, newImage);
        if(player.MiiImage != newImage) 
            player.MiiImage = newImage;
    }
    
    private static async Task<BitmapImage> GetMiiImageAsync(string base64MiiData)
    {
        using var formData = new MultipartFormDataContent();
        formData.Add(new ByteArrayContent(Convert.FromBase64String(base64MiiData)), "data", "mii.dat");
        formData.Add(new StringContent("wii"), "platform");
        var response = await HttpClientHelper.PostAsync<MiiResponse>(Endpoints.MiiStudioUrl, formData);
     
        if (!response.Succeeded || response.Content is null) return new BitmapImage();
        
        var miiImageUrl = GetMiiImageUrlFromResponse(response.Content);
        return new BitmapImage(new Uri(miiImageUrl));
    }

    private static string GetMiiImageUrlFromResponse(MiiResponse response)
    {
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
        var queryString = string.Join("&", queryParams);
        return $"{Endpoints.MiiImageUrl}?{queryString}";
    }

    private class MiiResponse
    {
        [JsonProperty("mii")]  
        public string? MiiData { get; set; }
        [JsonProperty("miistudio")]
        public string? MiiStudio { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("creator_name")] 
        public string? CreatorName { get; set; }
        public string? Birthday { get; set; }
        
        [JsonProperty("favorite_color")] 
        public string? FavoriteColor { get; set; }
        public int? Height { get; set; }
        public int? Build { get; set; }
        public string? Gender { get; set; }
        public string? Mingle { get; set; }
        public string? Copying { get; set; }
    }
}