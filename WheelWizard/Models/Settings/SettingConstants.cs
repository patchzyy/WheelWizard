using System;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Models.Settings;

public enum DolphinShaderCompilationMode {
    Default = 0,
    ExclusiveUberShaders = 1,
    HybridUberShaders = 2,
    SkipDrawing = 3
}

public static class SettingValues
{
    public static readonly double[] WindowScales = { 0.7, 0.8, 0.9, 1.0, 1.15, 1.3, 1.45 };
    
    public static readonly Dictionary<string, string> GFXRenderers = new()
    {
        { "DirectX 11 (Default)", "D3D11" },
        { "DirectX 12", "D3D12" },
        { "Vulkan", "Vulkan" },
        { "OpenGL", "OGL" }
    };
    

    public static readonly Dictionary<int, Func<string>> RrLanguages = new()
    {
        { 0 ,() => $"{Resources.Languages.Settings.Value_Language_English} ({Resources.Languages.Settings.Value_Language_EnglishOg})" }, // English
        { 1 ,() => $"{Resources.Languages.Settings.Value_Language_Japanese} ({Resources.Languages.Settings.Value_Language_JapaneseOg})" }, // Japanese
        { 2 ,() => $"{Resources.Languages.Settings.Value_Language_France} ({Resources.Languages.Settings.Value_Language_FranceOg})" }, // French
        { 3 ,() => $"{Resources.Languages.Settings.Value_Language_German} ({Resources.Languages.Settings.Value_Language_GermanOg})" }, // German
        { 4 ,() => $"{Resources.Languages.Settings.Value_Language_Dutch} ({Resources.Languages.Settings.Value_Language_DutchOg})" } // Dutch
    };

    public static readonly Dictionary<string, Func<string>> WhWzLanguages = new()
    {
        { "en", () => $"{Resources.Languages.Settings.Value_Language_English} ({Resources.Languages.Settings.Value_Language_EnglishOg})" },
        { "nl", () => $"{Resources.Languages.Settings.Value_Language_Dutch} ({Resources.Languages.Settings.Value_Language_DutchOg})" },
        { "fr", () => $"{Resources.Languages.Settings.Value_Language_France} ({Resources.Languages.Settings.Value_Language_FranceOg})" }
    };
}
   
