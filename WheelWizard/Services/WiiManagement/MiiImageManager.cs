using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WheelWizard.Helpers;
using WheelWizard.Models.RRInfo;

namespace WheelWizard.Services.WiiManagement;

public static class MiiImageManager
{
    private const int MaxCachedImages = 126;
    // There are always 2 values, the image itself, and a bool indicating if the image was loaded successfully
    // If it wasn't, it means that that image is just empty, yet it still should not be requested again since it failed
    private static readonly Dictionary<string, (Bitmap smallImage, bool success)> Images = new();
    private static readonly Queue<string> ImageOrder = new();
    public static int ImageCount { get; private set; } = 0;

    public static (Bitmap, bool)? GetCachedMiiImage(string miiData) => Images.TryGetValue(miiData, out var image) ? image : null;

    private static void AddMiiImage(string miiData, (Bitmap, bool) image)
    {
        if (!Images.ContainsKey(miiData))
            ImageOrder.Enqueue(miiData);
        Images[miiData] = image;

        ImageCount = Images.Count;
        if (Images.Count <= MaxCachedImages) return;
        var oldestMiiData = ImageOrder.Dequeue();
        Images.Remove(oldestMiiData);
        ImageCount = MaxCachedImages;
    }
    
    // Returns the image related to this data, if it is not cached, it will request it
    public static async Task<(Bitmap, bool)> LoadBase64MiiImageAsync(string base64MiiData)
    {
        if (string.IsNullOrEmpty(base64MiiData))
            return (CreateEmptyBitmap(), false);
        
        if (Images.ContainsKey(base64MiiData))
            return Images[base64MiiData];
        
        var newImage = await RequestMiiImageAsync(base64MiiData);
        AddMiiImage(base64MiiData, newImage);
        return newImage;
    }

    // Creates a new image of this Mii and adds it to the cache (if it was loaded successfully)
    public static async void ResetMiiImageAsync(Mii mii)
    {
        var miiData = mii.Data;
        var newImage = await RequestMiiImageAsync(miiData);
        AddMiiImage(miiData, newImage);
        if (mii.Image == newImage.Item1) return;

        mii.SetImage(newImage.Item1, newImage.Item2);
    }

    // Return the image, and a bool indicating if the image was loaded successfully
    private static async Task<(Bitmap, bool)> RequestMiiImageAsync(string base64MiiData)
    {
        using var formData = new MultipartFormDataContent();
        formData.Add(new ByteArrayContent(Convert.FromBase64String(base64MiiData)), "data", "mii.dat");
        formData.Add(new StringContent("wii"), "platform");

        var response = await HttpClientHelper.PostAsync<MiiResponse>(Endpoints.MiiStudioUrl, formData);

        if (!response.Succeeded || response.Content is null)
            return (CreateEmptyBitmap(), false);

        var miiImageUrl = GetMiiImageUrlFromResponse(response.Content);

        try
        {
            using var httpClient = new HttpClient();
            var imageStream = await httpClient.GetStreamAsync(miiImageUrl);
            var bitmap = new Bitmap(imageStream);
            return (bitmap, true);
        }
        catch
        {
            return (CreateEmptyBitmap(), false);
        }
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
    
    private static Bitmap CreateEmptyBitmap()
    {
        using var memoryStream = new MemoryStream();
        return new Bitmap(memoryStream);
    }
}
