using System.Text.Json.Serialization;

namespace CT_MKWII_WPF.Models.RRLaunchModels;

public class OptionConfig
{
    public int Choice { get; set; }

    [JsonPropertyName("option-name")]
    public string? OptionName { get; set; }

    [JsonPropertyName("section-name")]
    public string? SectionName { get; set; }
}