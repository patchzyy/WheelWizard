using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Services.Settings;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages.Settings;

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
        Button30FPS.IsChecked = (bool)SettingsManager.FORCE_30FPS.Get();
        ShowFPSButton.IsChecked = (bool)SettingsManager.SHOW_FPS.Get();
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
        foreach (var renderer in SettingValues.GFXRenderers.AllKeys)
        {
            RendererDropdown.Items.Add(renderer);
        }
        
        var currentRenderer = (string)SettingsManager.GFX_BACKEND.Get();
        var renderDisplayName = SettingValues.GFXRenderers.Mapping
                                       .FirstOrDefault(x => x.Value == currentRenderer).Key;
        if (renderDisplayName != null)
            RendererDropdown.SelectedItem = renderDisplayName;
        
        // -----------------
        // Language Dropdown
        // -----------------
        foreach (var language in SettingValues.Languages.AllKeys)
        {
            LanguageDropdown.Items.Add(language);
        }
        
        var currentLanguage = (int)SettingsManager.RR_LANGUAGE.Get();
        var languageDisplayName = SettingValues.Languages.Mapping
                                       .FirstOrDefault(x => x.Value == currentLanguage).Key;

        if (languageDisplayName != null)
            LanguageDropdown.SelectedItem = languageDisplayName;
    }

    private void UpdateResolution(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;

        SettingsManager.INTERNAL_RESOLUTION.Set(int.Parse(radioButton.Tag.ToString()!));
    }

    private void VSync_OnClick(object sender, RoutedEventArgs e) => SettingsManager.VSYNC.Set(VSyncButton.IsChecked == true);
    private void Recommended_OnClick(object sender, RoutedEventArgs e) => SettingsManager.RECOMMENDED_SETTINGS.Set(RecommendedButton.IsChecked == true);
    private void _30FPS_OnClick(object sender, RoutedEventArgs e) => SettingsManager.FORCE_30FPS.Set(Button30FPS.IsChecked == true);
    private void ShowFPS_OnClick(object sender, RoutedEventArgs e) => SettingsManager.SHOW_FPS.Set(ShowFPSButton.IsChecked == true);

    private void RendererDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedDisplayName = RendererDropdown.SelectedItem.ToString();
        if (SettingValues.GFXRenderers.Mapping.TryGetValue(selectedDisplayName, out var actualValue))
            SettingsManager.GFX_BACKEND.Set(actualValue);
        else
            MessageBox.Show($"Warning: Unknown renderer selected: {selectedDisplayName}");
    }
    
    private void LanguageDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedLanguage = LanguageDropdown.SelectedItem.ToString();
        if (SettingValues.Languages.Mapping.TryGetValue(selectedLanguage, out var actualValue))
            SettingsManager.RR_LANGUAGE.Set(actualValue);
        else
        {
            Console.WriteLine($"Warning: Unknown language selected: {selectedLanguage}");
        }
    }
}
