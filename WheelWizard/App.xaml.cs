using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Settings;
using System.Globalization;
using System.Windows;

namespace CT_MKWII_WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        SettingsManager.Instance.LoadSettings();
        AutoUpdater.CheckForUpdatesAsync();

        var lang = (string)SettingsManager.WW_LANGUAGE.Get();
        var newCulture = new CultureInfo(lang);
        CultureInfo.CurrentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;
    }
}
