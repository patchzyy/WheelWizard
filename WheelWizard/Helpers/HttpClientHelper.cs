using System;
using System.Net.Http;
using System.Threading.Tasks;
using CT_MKWII_WPF.Models;
using Newtonsoft.Json;

namespace CT_MKWII_WPF.Helpers;

public static class HttpClientHelper
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<HttpClientResult<T>> PostAsync<T>(string url, HttpContent? body)
    {
        HttpClientResult<T> result;
        try
        {
            var response = await HttpClient.PostAsync(url, body);
            result = new HttpClientResult<T>()
            {
                StatusCode = (int)response.StatusCode,
                Succeeded = response.IsSuccessStatusCode,
                StatusMessage = response.ReasonPhrase
            };

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (typeof(T) == typeof(string))
                    result.Content = (T)(object)content;
                else
                    result.Content = JsonConvert.DeserializeObject<T>(content);
            }
        }
        catch (Exception e)
        {
            result = GetErrorResult<T>(e);
        }

        return result;
    }

    public static async Task<HttpClientResult<T>> GetAsync<T>(string url)
    {
        HttpClientResult<T> result;
        try
        {
            var response = await HttpClient.GetAsync(url);
            result = new HttpClientResult<T>()
            {
                StatusCode = (int)response.StatusCode,
                Succeeded = response.IsSuccessStatusCode,
                StatusMessage = response.ReasonPhrase
            };

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (typeof(T) == typeof(string))
                    result.Content = (T)(object)content;
                else
                    result.Content = JsonConvert.DeserializeObject<T>(content);
            }
        }
        catch (Exception e)
        {
            result = GetErrorResult<T>(e);
        }

        return result;
    }

    private static HttpClientResult<T> GetErrorResult<T>(Exception e)
    {
        return new HttpClientResult<T>()
        {
            StatusCode = 500,
            StatusMessage = e.Message,
            Succeeded = false
        };
    }

    public static HttpClientResult<T> MockResult<T>(T content)
    {
        return new HttpClientResult<T>()
        {
            StatusCode = 200,
            StatusMessage = "OK",
            Succeeded = true,
            Content = content
        };
    }

    public static HttpClientResult<T> MockResult<T>(string content)
    {
        var result = new HttpClientResult<T>()
        {
            StatusCode = 200,
            StatusMessage = "OK",
            Succeeded = true
        };

        if (typeof(T) == typeof(string))
            result.Content = (T)(object)content;
        else
            result.Content = JsonConvert.DeserializeObject<T>(content);

        return result;
    }
}