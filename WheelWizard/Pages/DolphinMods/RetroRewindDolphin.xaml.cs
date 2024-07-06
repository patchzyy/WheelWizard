using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Utils;
using CT_MKWII_WPF.Utils.DolphinHelpers;

namespace CT_MKWII_WPF.Pages;

public partial class RetroRewindDolphin : UserControl
{
    public RetroRewindDolphin()
    {
        InitializeComponent();
        UpdateActionButton();
        UpdateResolutionDropDown();
        Loaded += (sender, e) => ResolutionDropDown.SelectionChanged += Change_Resolution;
        Loaded += (sender, e) => UpdateVSyncStatusButton();
        Loaded += (sender, e) => UpdateCurrentUberShaderStatus();
        
        VSyncCheckbox.Checked += Enable_VSync;
        VSyncCheckbox.Unchecked += Disable_VSync;
        ReccommendedSettingsCheckBox.Checked += EnableReccommendedSettings;
        ReccommendedSettingsCheckBox.Unchecked += DisableReccommendedSettings;
    }
    
    private void EnableReccommendedSettings(object sender, RoutedEventArgs e) 
        => DolphinSettingsUtils.EnableReccomendedSettings();
    
    
    private void DisableReccommendedSettings(object sender, RoutedEventArgs e) =>
        DolphinSettingsUtils.DisableReccommendedSettings(); 
    
    private void UpdateCurrentUberShaderStatus() =>
        ReccommendedSettingsCheckBox.IsChecked = DolphinSettingsUtils.IsReccommendedSettingsEnabled();
    
    private void UpdateVSyncStatusButton() =>
        VSyncCheckbox.IsChecked = DolphinSettingsUtils.GetCurrentVSyncStatus();
    

    private void UpdateResolutionDropDown() => 
        ResolutionDropDown.SelectedIndex = DolphinSettingsUtils.GetCurrentResolution() - 1;

    private async void UpdateActionButton()
    {
        RRStatusManager.ActionButtonStatus status = await RRStatusManager.GetCurrentStatus();
        switch (status)
        {
            case RRStatusManager.ActionButtonStatus.NoServer:
                ActionButton.Content = "No Server";
                ActionButton.IsEnabled = false;
                break;
            case RRStatusManager.ActionButtonStatus.NoDolphin:
                ActionButton.Content = "No Dolphin";
                ActionButton.IsEnabled = false;
                break;
            case RRStatusManager.ActionButtonStatus.ConfigNotFinished:
                ActionButton.Content = "Config Not Finished";
                ActionButton.IsEnabled = false;
                break;
            case RRStatusManager.ActionButtonStatus.noRR:
                ActionButton.Content = "Install Retro Rewind";
                ActionButton.IsEnabled = true;
                break;
            case RRStatusManager.ActionButtonStatus.noRRActive:
                //this is here for future use,
                //right now there is no de-activation, but if we want multiple mods this might be handy
                ActionButton.Content = "Activate Retro Rewind";
                ActionButton.IsEnabled = true;
                break;
            case RRStatusManager.ActionButtonStatus.RRnotReady:
                ActionButton.Content = "Activate Retro Rewind";
                ActionButton.IsEnabled = true;
                break;
            case RRStatusManager.ActionButtonStatus.OutOfDate:
                ActionButton.Content = "Update Retro Rewind";
                ActionButton.IsEnabled = true;
                break;
            case RRStatusManager.ActionButtonStatus.UpToDate:
                ActionButton.Content = "Play Retro Rewind";
                ActionButton.IsEnabled = true;
                break;
        }
    }

    private async void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        RRStatusManager.ActionButtonStatus status = await RRStatusManager.GetCurrentStatus();
        switch (status)
        {
            //fail save, but the button should be disabled
            case RRStatusManager.ActionButtonStatus.NoServer: break;
            case RRStatusManager.ActionButtonStatus.NoDolphin: break;
            case RRStatusManager.ActionButtonStatus.ConfigNotFinished: break;
            
            case RRStatusManager.ActionButtonStatus.noRR:
                await RetroRewindInstaller.InstallRetroRewind();
                UpdateActionButton();
                break;
            case RRStatusManager.ActionButtonStatus.noRRActive:
                //this is here for future use,
                //right now there is no de-activation, but if we want multiple mods this might be handy
                MessageBox.Show("Activate Retro Rewind");
                break;
            case RRStatusManager.ActionButtonStatus.RRnotReady:
                // RetroRewindInstaller.ActivateRetroRewind();
                break;
            case RRStatusManager.ActionButtonStatus.OutOfDate:
                await RetroRewindInstaller.UpdateRR();
                break;
            case RRStatusManager.ActionButtonStatus.UpToDate:
                RetroRewindLauncher.PlayRetroRewind();
                break;
        }
        UpdateActionButton();
    }

    private void Change_Resolution(object sender, SelectionChangedEventArgs e)
    {
        int Resolution = ResolutionDropDown.SelectedIndex + 1;
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Settings", "InternalResolution", Resolution.ToString());
        UpdateResolutionDropDown();
    }
    private void Enable_VSync(object sender, RoutedEventArgs e)
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Hardware", "VSync", "True");
    }
    
    private void Disable_VSync(object sender, RoutedEventArgs e)
    {
        DolphinSettingHelper.ChangeINISettings(SettingsUtils.FindGFXFile(), "Hardware", "VSync", "False");
    }
    
    private void TriggerNANDSetup(object sender, RoutedEventArgs e)
    {
        NANDTutorialUtils.RunNANDTutorial();
    }
}