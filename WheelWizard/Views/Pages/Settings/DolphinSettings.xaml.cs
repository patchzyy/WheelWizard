using CT_MKWII_WPF.Services.Settings;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages.Settings;

public partial class DolphinSettings : UserControl
{
    public DolphinSettings()
    {
        InitializeComponent();
    }

    private void ClickForceWiimote(object sender, RoutedEventArgs e)
    {
        SettingsManager.FORCE_WIIMOTE.Set(DisableForce.IsChecked == true);
    }

    private void ClickLaunchWithDolphinWindow(object sender, RoutedEventArgs e)
    {
        SettingsManager.LAUNCH_WITH_DOLPHIN.Set(LaunchWithDolphin.IsChecked == true);
    }
}

