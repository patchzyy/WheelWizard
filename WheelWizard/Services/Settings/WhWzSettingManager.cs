using CT_MKWII_WPF.Models.Settings;

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using JsonElement = System.Text.Json.JsonElement;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace CT_MKWII_WPF.Services.Settings;

public static class WhWzSettingManager
{
    private static readonly string TestPath = // fill your path in here
    private static bool _loaded;
    
    public static void SaveSettings(Setting invokingSetting)
    {
        if (!_loaded) return;
        var whWzSettings = SettingsManager.Instance.GetWhWzSettings();
        
        var settingsToSave = new Dictionary<string, object?>();
    
        foreach (var (name,setting) in whWzSettings)
        {
            settingsToSave[name] = setting.Get();
        }
        var jsonString = JsonSerializer.Serialize(settingsToSave, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(TestPath, jsonString);
    }
    
    public static void LoadSettings(Dictionary<string, WhWzSetting> settings)
    {
        if (_loaded) return;
        
        if (!File.Exists(TestPath)) 
            return;
    
        var jsonString = File.ReadAllText(TestPath);
        var loadedSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);
        if (loadedSettings == null) 
            return;
        
        foreach (var kvp in loadedSettings)
        {
            if (!settings.TryGetValue(kvp.Key, out var setting))
                continue;
            
            setting.Set(kvp.Value);
        }
        
        _loaded = true;
    }
}
