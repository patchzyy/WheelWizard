using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Settings;
using System;
using System.Windows;

namespace CT_MKWII_WPF;

public partial class App : Application
{
    public enum MyEnum
    {
        HEY,
        OTHER,
        WRONG
    };
    
    protected override void OnStartup(StartupEventArgs e)
    {
        SettingsManager.Instance.LoadSettings();
        AutoUpdater.CheckForUpdatesAsync();
        
        Console.WriteLine(SettingsManager.DOLHPIN_CORE.Get());
        SettingsManager.DOLHPIN_CORE.Set(MyEnum.OTHER);
        Console.WriteLine(SettingsManager.DOLHPIN_CORE.Get());
    }
}
