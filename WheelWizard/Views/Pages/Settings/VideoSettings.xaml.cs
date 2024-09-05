using CT_MKWII_WPF.Services.Settings;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages.Settings;

public partial class VideoSettings : UserControl
{
    public VideoSettings()
    {
        InitializeComponent();
    }

    private void UpdateResolution(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton) 
            return;
        
        SettingsManager.INTERNAL_RESOLUTION.Set(int.Parse(radioButton.Tag.ToString()!));
    }

    private void VSync_OnClick(object sender, RoutedEventArgs e) => SettingsManager.VSYNC.Set(VSyncButton.IsChecked == true);
    private void Recommended_OnClick(object sender, RoutedEventArgs e) => SettingsManager.RECOMMENDED_SETTINGS.Set(RecommendedButton.IsChecked == true);
    private void _30FPS_OnClick(object sender, RoutedEventArgs e)
    {
        SettingsManager.FORCE_30FPS.Set(Button30FPS.IsChecked == true);
    }
    
    private void ShowFPS_OnClick(object sender, RoutedEventArgs e)
    {
        SettingsManager.SHOW_FPS.Set(ShowFPSButton.IsChecked == true);
    }
}

