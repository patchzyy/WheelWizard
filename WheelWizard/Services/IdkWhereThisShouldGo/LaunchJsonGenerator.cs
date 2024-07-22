using CT_MKWII_WPF.Models.RRLaunchModels;
using CT_MKWII_WPF.Services.Settings;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CT_MKWII_WPF.Services.IdkWhereThisShouldGo;

public static class LaunchJsonGenerator
{
    private const string JsonFileName = "RR.json";
    private const string XmlFileName = "RetroRewind6.xml";

    public static void GenerateLaunchJson(bool launchTt)
    {
        var launchConfig = new LaunchConfig
        {
            BaseFile = PathManager.GetGameLocation(),
            DisplayName = "RR",
            Riivolution = new RiivolutionConfig
            {
                Patches = new[]
                {
                    new PatchConfig
                    {
                        Options = new[]
                        {
                            new OptionConfig { Choice = 1, OptionName = "Pack", SectionName = "Retro Rewind" },
                            new OptionConfig { Choice = 2, OptionName = "My Stuff", SectionName = "Retro Rewind" },
                            new OptionConfig { Choice = launchTt ? 1 : 0, OptionName = "Online TT", SectionName = "Retro Rewind" }
                        },
                        Root = Path.Combine(PathManager.GetLoadPathLocation(), "Riivolution"),
                        Xml = Path.Combine(PathManager.GetLoadPathLocation(), "Riivolution", "riivolution", XmlFileName)
                    }
                }
            },
            Type = "dolphin-game-mod-descriptor",
            Version = 1
        };

        var jsonString = JsonSerializer.Serialize(launchConfig, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        var outputPath = Path.Combine(
            ConfigManager.GetWheelWizardAppdataPath(),
            JsonFileName
        );

        File.WriteAllText(outputPath, jsonString);
    }
}
