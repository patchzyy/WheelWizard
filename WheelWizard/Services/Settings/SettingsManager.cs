using CT_MKWII_WPF.Models.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CT_MKWII_WPF.Services.Settings;

// TODO: 
// - Add a new Setting type "DolphinSetting" which has 2 extra names, the FileName and the SectionName
// - Test this new Setting type thoroughly with a fake ini file before trying it on the real data
// - Add a new Setting type "VirtualSetting", this setting will not actually save any data, but has a lambda for getting and setting data
//      This is e.g. used for the Recommended setting,  which instead gets and sets multiple DolphinSettings
//      Do a little research before you implement this, since maybe you dont even have to do this using lambda's,
//      but instead yuo could create another class like "GroupSetting" which just has a bunch of Settings and their value on both toggles
//      However, if you make such a setting, it should probably be limited to bools and enums.
//      You probably just need to make a Virtual setting and extend that with this Groupseting that only allows bools and enums

public class SettingsManager
{
    private readonly Dictionary<string, Setting> _settings = new();
    public static SettingsManager Instance { get; } = new();
    
    private SettingsManager()
    {
        AddSettings(
            new WhWzSetting(typeof(bool),"HasRunNANDTutorial", false), // Not sure if this is needed anymore
            new WhWzSetting(typeof(bool),"ForceWiimote", false),
            new WhWzSetting(typeof(string),"DolphinLocation", "")
                .SetValidation(value => File.Exists(value as string ?? string.Empty)),
            new WhWzSetting(typeof(string),"GameLocation", "")
                .SetValidation(value => File.Exists(value as string ?? string.Empty)),
            new WhWzSetting(typeof(string),"UserFolderPath", "")
                .SetValidation(value => Directory.Exists(value as string ?? string.Empty))
            );
        
        LoadSettings();
    }

    public void Test()
    {
     
    }
    
    private void AddSettings(params Setting[] settings)
    {
        foreach (var s in settings)
        {
            _settings.Add(s.Name, s);
        }
    }
    
    private void LoadSettings()
    {
        WhWzSettingManager.LoadSettings(GetWhWzSettings());
    }
    
    public Setting GetSetting(string name) => _settings[name];
    public bool SetSetting(string name, object value) => _settings[name].Set(value);
    public void ResetSetting(string name) => _settings[name].Reset();
    
    public Dictionary<string, Setting> GetSettings() => _settings;

    public Dictionary<string, WhWzSetting> GetWhWzSettings()
    {
        return _settings.Where(kv => kv.Value is WhWzSetting)
                        .ToDictionary(kv => kv.Key, kv => (WhWzSetting)kv.Value);
    }
}
