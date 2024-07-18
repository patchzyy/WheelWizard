using System.Text.Json.Serialization;

namespace CT_MKWII_WPF.Models.RRLaunchModels;

public class LaunchConfig
{
    [JsonPropertyName("base-file")]
    public string? BaseFile { get; set; }

    [JsonPropertyName("display-name")]
    public string? DisplayName { get; set; }

    public RiivolutionConfig? Riivolution { get; set; }

    public string? Type { get; set; }

    public int? Version { get; set; }
}