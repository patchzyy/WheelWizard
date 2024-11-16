using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WheelWizard.Helpers;
using WheelWizard.Models.Settings;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.Settings;
using WheelWizard.WPFViews.Popups.Generic;

namespace WheelWizard.WPFViews.Pages.Settings;

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
        foreach (var lang in SettingValues.RrLanguages.Values)
        {
            InGameLanguageDropdown.Items.Add(lang());
        }
        
        var currentRrLanguage = (int)SettingsManager.RR_LANGUAGE.Get();
        var rrLanguageDisplayName = SettingValues.RrLanguages[currentRrLanguage];
        InGameLanguageDropdown.SelectedItem = rrLanguageDisplayName();
        
        // -----------------
        // Wheel Wizard Language Dropdown
        // -----------------
        foreach (var lang in SettingValues.WhWzLanguages.Values)
        {
            WhWzLanguageDropdown.Items.Add(lang());
        }
        
        var currentWhWzLanguage = (string)SettingsManager.WW_LANGUAGE.Get();
        var whWzLanguageDisplayName = SettingValues.WhWzLanguages[currentWhWzLanguage];
        WhWzLanguageDropdown.SelectedItem = whWzLanguageDisplayName();

        if (WheelWizard.Resources.Languages.Settings.CompletePercentage != "100")
        {
            TranslationsPercentageText.Visibility = Visibility.Visible;
            var percentage = WheelWizard.Resources.Languages.Settings.CompletePercentage;
            var phrase = Phrases.Text_WhWzTranslationPercentage;
            TranslationsPercentageText.Text = Humanizer.ReplaceDynamic(phrase, percentage);
        }
    }

    private void ClickForceWiimote(object sender, RoutedEventArgs e) => SettingsManager.FORCE_WIIMOTE.Set(DisableForce.IsChecked == true);
    private void ClickLaunchWithDolphinWindow(object sender, RoutedEventArgs e) => SettingsManager.LAUNCH_WITH_DOLPHIN.Set(LaunchWithDolphin.IsChecked == true);
    
    private void RrLanguageDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedLanguage = InGameLanguageDropdown.SelectedItem.ToString();
        var key = SettingValues.RrLanguages.FirstOrDefault(x => x.Value() == selectedLanguage).Key;
            SettingsManager.RR_LANGUAGE.Set(key);
    }
    
    private void WhWzLanguageDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedLanguage = WhWzLanguageDropdown.SelectedItem.ToString();
        var key = SettingValues.WhWzLanguages
                               .FirstOrDefault(x => x.Value() == selectedLanguage).Key;
        
        var currentLanguage = (string)SettingsManager.WW_LANGUAGE.Get();
        if (key == null || key == currentLanguage ) 
            return;
        
        var yesNoWindow = new YesNoWindow()
                          .SetMainText("Do you want to apply the new language settings?")
                          .SetExtraText("This will close the current window and open a new one with the new language settings.")
                          .SetButtonText(Common.Action_Apply, Common.Action_Cancel);
        if (!yesNoWindow.AwaitAnswer())
        {
            var currentWhWzLanguage = (string)SettingsManager.WW_LANGUAGE.Get();
            var whWzLanguageDisplayName = SettingValues.WhWzLanguages[currentWhWzLanguage];
            WhWzLanguageDropdown.SelectedItem = whWzLanguageDisplayName();
            return; // We only want to change the setting if we really apply this change
        }
        
        SettingsManager.WW_LANGUAGE.Set(key);
        ViewUtils.RefreshWindow( new SettingsPage(new OtherSettings()) );
    }
}
