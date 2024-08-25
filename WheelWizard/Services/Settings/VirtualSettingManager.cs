using CT_MKWII_WPF.Models.Settings;
using System.Collections.Generic;

namespace CT_MKWII_WPF.Services.Settings;

public class VirtualSettingManager
{
    private bool _loaded;
    private readonly List<VirtualSetting> _settings = new();
    
    public static VirtualSettingManager Instance { get; } = new();
    private VirtualSettingManager() { }
    
    public void RegisterSetting(VirtualSetting setting)
    {
        if (_loaded) 
            return;
        
        _settings.Add(setting);
    }
    
    // Nothing we have to do now, but we might have to do something in the future
    public void SaveSettings(VirtualSetting invokingSetting) { }
    
    public void LoadSettings()
    {
        if (_loaded) 
            return;
        
        foreach (var setting in _settings)
        {
            setting.Recalculate();
        }
        
        _loaded = true;
    }
}
