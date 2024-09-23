using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages.Settings;

public partial class OtherSettings : UserControl
{
    private readonly bool _settingsAreDisabled;
        
    public OtherSettings()
    {
        InitializeComponent();
        _settingsAreDisabled = !SettingsHelper.PathsSetupCorrectly();
        DisabledWarningText.Visibility = _settingsAreDisabled ? Visibility.Visible : Visibility.Collapsed;
        
        DolphinBorder.IsEnabled = !_settingsAreDisabled;
        if (!_settingsAreDisabled)
            LoadSettings();
        ForceLoadSettings();
    }

    private void LoadSettings()
    {
        DisableForce.IsChecked = (bool)SettingsManager.FORCE_WIIMOTE.Get();
        LaunchWithDolphin.IsChecked = (bool)SettingsManager.LAUNCH_WITH_DOLPHIN.Get();
    }

    private void ForceLoadSettings()
    {
        // -----------------
        // Retro Rewind Language Dropdown
        // -----------------
        foreach (var lang in SettingValues.RrLanguages.Keys)
        {
            InGameLanguageDropdown.Items.Add(lang);
        }
        
        var currentRrLanguage = (int)SettingsManager.RR_LANGUAGE.Get();
        var rrLanguageDisplayName = SettingValues.RrLanguages
                                               .FirstOrDefault(x => x.Value == currentRrLanguage).Key;

        if (rrLanguageDisplayName != null)
            InGameLanguageDropdown.SelectedItem = rrLanguageDisplayName;
        
        // -----------------
        // Wheel Wizard Language Dropdown
        // -----------------
        foreach (var lang in SettingValues.WhWzLanguages.Keys)
        {
            WhWzLanguageDropdown.Items.Add(lang);
        }
        
        var currentWhWzLanguage = (string)SettingsManager.WW_LANGUAGE.Get();
        var whWzLanguageDisplayName = SettingValues.WhWzLanguages
                                               .FirstOrDefault(x => x.Value == currentWhWzLanguage).Key;

        if (whWzLanguageDisplayName != null)
            WhWzLanguageDropdown.SelectedItem = whWzLanguageDisplayName;
    }

    private void ClickForceWiimote(object sender, RoutedEventArgs e) => SettingsManager.FORCE_WIIMOTE.Set(DisableForce.IsChecked == true);
    private void ClickLaunchWithDolphinWindow(object sender, RoutedEventArgs e) => SettingsManager.LAUNCH_WITH_DOLPHIN.Set(LaunchWithDolphin.IsChecked == true);
    
    private void RrLanguageDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedLanguage = InGameLanguageDropdown.SelectedItem.ToString();
        if (SettingValues.RrLanguages.TryGetValue(selectedLanguage!, out var actualValue))
            SettingsManager.RR_LANGUAGE.Set(actualValue);
    }
    
    private void WhWzLanguageDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedLanguage = WhWzLanguageDropdown.SelectedItem.ToString();
        var currentLanguage = (string)SettingsManager.WW_LANGUAGE.Get();
        if (!SettingValues.WhWzLanguages.TryGetValue(selectedLanguage!, out var actualValue) ||
            actualValue == currentLanguage) 
            return;
        
        var yesNoWindow = new YesNoWindow()
                          .SetMainText("Do you want to apply the new language settings?")
                          .SetExtraText("This will close the current window and open a new one with the new language settings.")
                          .SetButtonText("Apply", "Cancel");
        if (!yesNoWindow.AwaitAnswer())
        {
            var currentWhWzLanguage = (string)SettingsManager.WW_LANGUAGE.Get();
            var whWzLanguageDisplayName = SettingValues.WhWzLanguages
                                                       .FirstOrDefault(x => x.Value == currentWhWzLanguage).Key;
            if (whWzLanguageDisplayName != null) WhWzLanguageDropdown.SelectedItem = whWzLanguageDisplayName;
            return; // We only want to change the setting if we really apply this change
        }
        
        SettingsManager.WW_LANGUAGE.Set(actualValue);
        ViewUtils.RefreshWindow( new SettingsPage(new OtherSettings()) );
    }
}
