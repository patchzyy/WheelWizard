using CT_MKWII_WPF.Models.RRLaunchModels;
using CT_MKWII_WPF.Services.Settings;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CT_MKWII_WPF.Services.Launcher;

public static class RetroRewindLaunchHelper
{
    private static string RootRiivolution => Path.Combine(PathManager.GetLoadPathLocation(), "Riivolution");
    private static string XmlPath => Path.Combine(RootRiivolution, "riivolution", "RetroRewind6.xml");
    private static string JsonPath => Path.Combine(ConfigManager.GetWheelWizardAppdataPath(), "RR.json");
    
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
                        Root = RootRiivolution,
                        Xml = XmlPath
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
        
        File.WriteAllText(JsonPath, jsonString);
    }
}
