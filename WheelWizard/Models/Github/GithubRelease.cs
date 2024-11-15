using System;
using System.Text.Json.Serialization;

namespace CT_MKWII_WPF.Models.Github;

public class GithubRelease
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }
    [JsonPropertyName("assets")]
    public GithubAsset[] Assets { get; set; }
    [JsonPropertyName("body")]
    public string Body { get; set; }
    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
}

public class GithubAsset
{
    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; }
    
    [JsonPropertyName("uploader")]
    public GithubUploader Uploader { get; set; }
}

public class GithubUploader
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
