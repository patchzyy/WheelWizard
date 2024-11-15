using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WheelWizard.Models.Settings;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.Settings;
using WheelWizard.Views.Popups;

namespace WheelWizard.Views.Pages.Settings;

public partial class VideoSettings : UserControl
{
    private readonly bool _settingsAreDisabled;
    
    public VideoSettings()
    {
        InitializeComponent();
        _settingsAreDisabled = !SettingsHelper.PathsSetupCorrectly();
        DisabledWarningText.Visibility = _settingsAreDisabled ? Visibility.Visible : Visibility.Collapsed;
        VideoBorder.IsEnabled = !_settingsAreDisabled;
        
        if (!_settingsAreDisabled)
            LoadSettings();
        ForceLoadSettings();
    }

    private void LoadSettings()
    {
        // These are all the settings that are loaded when the settings page is opened.
        // Note that all the settings in this method only load when editing settings is enabled.
        VSyncButton.IsChecked = (bool)SettingsManager.VSYNC.Get();
        RecommendedButton.IsChecked = (bool)SettingsManager.RECOMMENDED_SETTINGS.Get();
      
        ShowFPSButton.IsChecked = (bool)SettingsManager.SHOW_FPS.Get();
        RemoveBlurButton.IsChecked = (bool)SettingsManager.REMOVE_BLUR.Get();
        var finalResolution = (int)SettingsManager.INTERNAL_RESOLUTION.Get();
        if (finalResolution < 0 || finalResolution > ResolutionStackPanel.Children.Count)
            return;
   
        foreach (var element in ResolutionStackPanel.Children)
        {
            if (element is not RadioButton radioButton)
                continue;
            
            radioButton.IsChecked = (radioButton.Tag.ToString() == finalResolution.ToString());
        }
    }
 
    private void ForceLoadSettings()
    {
        // These are all the settings that are loaded when the settings page is opened.
        // Note that all the settings in this method always load, regardless of editing settings being enabled.
        // Its not really nessesairy for the app to work, but we want to at least show the user what the current settings are,
        // even though they can't change them.
        
        // -----------------
        // Render Dropdown
        // -----------------
        foreach (var renderer in SettingValues.GFXRenderers.Keys)
        {
            RendererDropdown.Items.Add(renderer);
        }
        
        var currentRenderer = (string)SettingsManager.GFX_BACKEND.Get();
        var renderDisplayName = SettingValues.GFXRenderers
                                       .FirstOrDefault(x => x.Value == currentRenderer).Key;
        if (renderDisplayName != null)
            RendererDropdown.SelectedItem = renderDisplayName;
    }

    private void UpdateResolution(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;

        SettingsManager.INTERNAL_RESOLUTION.Set(int.Parse(radioButton.Tag.ToString()!));
    }

    private void VSync_OnClick(object sender, RoutedEventArgs e) => SettingsManager.VSYNC.Set(VSyncButton.IsChecked == true);
    private void Recommended_OnClick(object sender, RoutedEventArgs e) => SettingsManager.RECOMMENDED_SETTINGS.Set(RecommendedButton.IsChecked == true);
    private void ShowFPS_OnClick(object sender, RoutedEventArgs e) => SettingsManager.SHOW_FPS.Set(ShowFPSButton.IsChecked == true);
    private void RemoveBlur_OnClick(object sender, RoutedEventArgs e) => SettingsManager.REMOVE_BLUR.Set(RemoveBlurButton.IsChecked == true);

    private void RendererDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedDisplayName = RendererDropdown.SelectedItem.ToString();
        if (SettingValues.GFXRenderers.TryGetValue(selectedDisplayName, out var actualValue))
            SettingsManager.GFX_BACKEND.Set(actualValue);
        else
            MessageBoxWindow.ShowDialog($"{Common.Term_Warning}: Unknown renderer selected: {selectedDisplayName}");
    }
}
