using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.Settings;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using JsonElement = System.Text.Json.JsonElement;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace CT_MKWII_WPF.Services.Settings;

public class WhWzSettingManager
{
    private static string ConfigFilePath => Path.Combine(PathManager.WheelWizardAppdataPath, "test.json");
    private bool _loaded;
    private readonly Dictionary<string, WhWzSetting> _settings = new();
    
    public static WhWzSettingManager Instance { get; } = new();
    private WhWzSettingManager() { }
    
    public void RegisterSetting(WhWzSetting setting)
    {
        if (_loaded) 
            return;
        
        _settings.Add(setting.Name, setting);
    }
    
    public void SaveSettings(WhWzSetting invokingSetting)
    {
        if (!_loaded) 
            return;
        
        var settingsToSave = new Dictionary<string, object?>();
    
        foreach (var (name, setting) in _settings)
        {
            settingsToSave[name] = setting.Get();
        }
        var jsonString = JsonSerializer.Serialize(settingsToSave, new JsonSerializerOptions { WriteIndented = true });
        FileHelper.WriteAllTextSafe(ConfigFilePath, jsonString);
    }
    
    public void LoadSettings()
    {
        if (_loaded) 
            return;

        _loaded = true;
        // even if it will now return early, that means its still done loading since then there is nothing to load
        var jsonString = FileHelper.ReadAllTextSafe(ConfigFilePath);
        if (jsonString == null)
            return;
        
        var loadedSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);
        if (loadedSettings == null) 
            return;
        
        foreach (var kvp in loadedSettings)
        {
            if (!_settings.TryGetValue(kvp.Key, out var setting))
                continue;
            
            setting.SetFromJson(kvp.Value);
        }
    }
}
