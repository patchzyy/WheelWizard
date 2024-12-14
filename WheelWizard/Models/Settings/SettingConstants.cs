using System;
using System.Collections.Generic;
using System.Windows;

namespace WheelWizard.Models.Settings;

public enum DolphinShaderCompilationMode {
    Default = 0,
    ExclusiveUberShaders = 1,
    HybridUberShaders = 2,
    SkipDrawing = 3
}

public static class SettingValues
{
    // These should not be seen, but are instead a placeholder for values. When you then use them to display something
    // you check for this value and replace it with its corresponding value in the language file
    public const string NoName = "no name";
    public const string NoLicense = "no license";
    
    public static readonly double[] WindowScales = { 0.7, 0.8, 0.9, 1.0, 1.15, 1.3, 1.45 };
    
    public static readonly Dictionary<string, string> GFXRenderers = new()
    {
        { "DirectX 11 (Default)", "D3D11" },
        { "DirectX 12", "D3D12" },
        { "Vulkan", "Vulkan" },
        { "OpenGL", "OGL" }
    };
    
    public static readonly Dictionary<int,Func<string>> RrLanguages = new()
    {
        { 0, () => CreateLanguageString("English") },
        { 1, () => CreateLanguageString("Japanese") },
        { 2, () => CreateLanguageString("France") },
        { 3, () => CreateLanguageString("German") },
        { 4, () => CreateLanguageString("Dutch") },
        { 5, () => CreateLanguageString("Spanish") },
        { 6, () => CreateLanguageString("Finnish") }
    };

    public static readonly Dictionary<string, Func<string>> WhWzLanguages = new()
    {
        { "en", () => CreateLanguageString("English") },
        { "nl", () => CreateLanguageString("Dutch") },
        { "fr", () => CreateLanguageString("France") },
        { "de", () => CreateLanguageString("German") },
        { "ja", () => CreateLanguageString("Japanese") },
        { "es", () => CreateLanguageString("Spanish") },
        { "it", () => CreateLanguageString("Italian") },
        { "ru", () => CreateLanguageString("Russian") }
    };
    
    private static string CreateLanguageString(string language)
    {
        var lang = Resources.Languages.Settings.ResourceManager.GetString($"Value_Language_{language}")!; 
        var langOg =  Resources.Languages.Settings.ResourceManager.GetString($"Value_Language_{language}Og");
        if (lang == langOg || langOg == null || langOg == "-")
            return lang;
        
        return $"{lang} - ({langOg})";
    }
}
