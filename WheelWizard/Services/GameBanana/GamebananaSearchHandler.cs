using CT_MKWII_WPF.Models.GameBanana;
using System;
using System.Net;

namespace CT_MKWII_WPF.Services.GameBanana;

using System.Threading.Tasks;
using Helpers;
using Models;

public class GamebananaSearchHandler
{
    public const string BaseUrl = Endpoints.GameBananaBaseUrl;
    public const int GAME_ID = 5896;
    
    public static async Task<HttpClientResult<GameBananaResponse>> SearchModsAsync(string searchString, int page = 1, int perPage = 20)
    {
        if (string.IsNullOrWhiteSpace(searchString)) searchString = "mod";
        if (page < 1) page = 1;
        if (perPage < 1 || perPage > 50) perPage = 20;
        var searchUrl = $"{BaseUrl}/Util/Search/Results?_sSearchString={searchString}&_nPage={page}&_nPerpage={perPage}&_idGameRow={GAME_ID}";
        Console.WriteLine("Request URL: " + searchUrl);
        var result = await HttpClientHelper.GetAsync<GameBananaResponse>(searchUrl);

        if (!result.Succeeded)
        {
            Console.WriteLine($"Error: {result.StatusMessage} (HTTP {result.StatusCode})");
        }

        return result;
    }
    
    public static async Task<HttpClientResult<ModDetailResponse>> GetModDetailsAsync(int modId)
    {
        var modDetailUrl = $"{BaseUrl}/Mod/{modId}/ProfilePage";
        Console.WriteLine("Mod Detail URL: " + modDetailUrl);
        var result = await HttpClientHelper.GetAsync<ModDetailResponse>(modDetailUrl);

        if (!result.Succeeded)
        {
            Console.WriteLine($"Error fetching mod details: {result.StatusMessage} (HTTP {result.StatusCode})");
        }

        return result;
    }
    
    public static async Task<HttpClientResult<GameBananaResponse>> GetFeaturedModsAsync()
    {
        var featuredUrl = $"{BaseUrl}/Util/List/Featured?_idGameRow={GAME_ID}";
        return await HttpClientHelper.GetAsync<GameBananaResponse>(featuredUrl);
    }

    public static async Task<HttpClientResult<GameBananaResponse>> GetLatestModsAsync(int page = 1)
    {
        if (page < 1) page = 1;
        var latestModsUrl = $"{BaseUrl}/Game/{GAME_ID}/Subfeed?_nPage={page}";
        return await HttpClientHelper.GetAsync<GameBananaResponse>(latestModsUrl);
    }
}
