using System.Text.Json.Serialization;

namespace CT_MKWII_WPF.Models.Github;

public class GithubRelease
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }
    [JsonPropertyName("assets")]
    public GithubAsset[] Assets { get; set; }
}

public class GithubAsset
{
    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; }
}
