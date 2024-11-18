using MahApps.Metro.IconPacks;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WheelWizard.Models.Enums;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.Installation;
using WheelWizard.Services.Launcher;
using WheelWizard.Services.Other;
using WheelWizard.Services.Settings;
using WheelWizard.WPFViews.Pages.Settings;
using static WheelWizard.WPFViews.ViewUtils;
using Button = WheelWizard.WPFViews.Components.Button;
using Components_Button = WheelWizard.WPFViews.Components.Button;

namespace WheelWizard.WPFViews.Pages;

public partial class HomePage
{
    private readonly DoubleAnimation _fastRotationAnimation;
    private readonly DoubleAnimation _defaultRotationAnimation;
    private bool _isSpeedBoostActive;
    
    public HomePage()
    {
        InitializeComponent();
        UpdateActionButton();
        
        _fastRotationAnimation = new DoubleAnimation
        {
            From = 0, To = 360,
            Duration = TimeSpan.FromSeconds(1),
            RepeatBehavior = RepeatBehavior.Forever
        };
        
        _defaultRotationAnimation = new DoubleAnimation
        {
            From = 0, To = 360,
            Duration = TimeSpan.FromSeconds(15), 
            RepeatBehavior = RepeatBehavior.Forever
        };
        
        if ((bool)SettingsManager.ENABLE_ANIMATIONS.Get())
            WheelIcon.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, _defaultRotationAnimation);
    }
    
    private WheelWizardStatus _status;

    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        var status = _status;
        switch (status)
        {
            case WheelWizardStatus.NoServer:
                break;
            case WheelWizardStatus.NoServerButInstalled:
                await Launcher.LaunchRetroRewind();
                break;
            case WheelWizardStatus.NoDolphin:
                NavigateToPage(new SettingsPage());
                break;
            case WheelWizardStatus.ConfigNotFinished:
                NavigateToPage(new SettingsPage());
                break;
            case WheelWizardStatus.NoRR:
                SetButtonState(Common.State_Installing, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download,
                    false);
                DisableSidebarButtons();
                await RetroRewindInstaller.InstallRetroRewind();
                EnableSidebarButtons();
                break;
            case WheelWizardStatus.RRNotReady:
                SetButtonState("Activating...", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Power, false);
                DisableSidebarButtons();
                await RetroRewindInstaller.InstallRetroRewind();
                EnableSidebarButtons();
                break;
            case WheelWizardStatus.OutOfDate:
                SetButtonState(Common.State_Updating, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Update, false);
                DisableSidebarButtons();
                await RetroRewindUpdater.UpdateRR();
                EnableSidebarButtons();
                break;
            case WheelWizardStatus.UpToDate:
                SpeedBoostWheel();
                await Launcher.LaunchRetroRewind();
                break;
        }
        UpdateActionButton();
        DisableAllButtonsTemporarily();
    }

    private async void UpdateActionButton()
    {
        SetButtonState(Common.State_Loading, Button.ButtonsVariantType.Secondary, 
                       PackIconFontAwesomeKind.SpinnerSolid, false);
        _status = await StatusManager.GetCurrentStatus();
        switch (_status)
        {
            case WheelWizardStatus.NoServer:
                SetButtonState(Common.State_NoServer, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.ServerNetworkOff);
                break;
            case WheelWizardStatus.NoServerButInstalled:
                SetButtonState(Common.Action_PlayOffline, Button.ButtonsVariantType.Secondary, PackIconFontAwesomeKind.PlaySolid);
                break;
            case WheelWizardStatus.NoDolphin:
                SetButtonState("Settings", Button.ButtonsVariantType.Secondary, PackIconFontAwesomeKind.FilePenSolid);
                break;
            case WheelWizardStatus.ConfigNotFinished:
                SetButtonState(Common.State_ConfigNotFinished, Button.ButtonsVariantType.Secondary,
                    PackIconFontAwesomeKind.FilePenSolid);
                break;
            case WheelWizardStatus.NoRR:
                SetButtonState(Common.Action_Install, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download);
                break;
            case WheelWizardStatus.NoRRActive:
                //this is here for future use,
                //right now there is no de-activation, but if we want multiple mods this might be handy
                SetButtonState("Activated", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Power);
                break;
            case WheelWizardStatus.RRNotReady:
                SetButtonState("Activate", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Power);
                break;
            case WheelWizardStatus.OutOfDate:
                SetButtonState(Common.Action_Update, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download);
                break;
            case WheelWizardStatus.UpToDate:
                SetButtonState(Common.Action_Play, Button.ButtonsVariantType.Primary, PackIconFontAwesomeKind.PlaySolid);
                break;
        }
        if (SettingsHelper.PathsSetupCorrectly()) 
            return;
        
        DolphinButton.IsEnabled = false;
    }

    private void SetButtonState(string text, Components_Button.ButtonsVariantType variant, PackIconMaterialKind iconKind,
        bool enabled = true, bool subButtonsEnabled = true)
    {
        PlayButton.Text = text;
        PlayButton.Variant = variant;
        PlayButton.IsEnabled = enabled;
        PlayButton.IconPack = "Material";
        PlayButton.IconKind = iconKind;
        // MiiButton.IsEnabled = subButtonsEnabled;
        DolphinButton.IsEnabled = subButtonsEnabled;
    }

    private void SetButtonState(string text, Components_Button.ButtonsVariantType variant, PackIconFontAwesomeKind iconKind,
        bool enabled = true, bool subButtonsEnabled = true)
    {
        PlayButton.Text = text;
        PlayButton.Variant = variant;
        PlayButton.IsEnabled = enabled;
        PlayButton.IconPack = "FontAwesome";
        PlayButton.IconKind = iconKind;
        // MiiButton.IsEnabled = subButtonsEnabled;
        DolphinButton.IsEnabled = subButtonsEnabled;
    }

    private void DisableSidebarButtons() => GetLayout().SidePanelButtons.IsEnabled = false;
    private void EnableSidebarButtons() => GetLayout().SidePanelButtons.IsEnabled = true;

    private void DolphinButton_OnClick(object sender, RoutedEventArgs e)
    {
        Launcher.LaunchDolphin();
        DisableAllButtonsTemporarily();
    }

    private void MiiButton_OnClick(object sender, RoutedEventArgs e)
    {
        _ = Launcher.LaunchMiiChannel();
        DisableAllButtonsTemporarily();
    }

    private void DisableAllButtonsTemporarily()
    {
        CompleteGrid.IsEnabled = false;
        //wait 5 seconds before re-enabling the buttons
        Task.Delay(5000).ContinueWith(_ => { Dispatcher.Invoke(() => CompleteGrid.IsEnabled = true); });
    }

    private async void SpeedBoostWheel()
    {
        if (!(bool)SettingsManager.ENABLE_ANIMATIONS.Get()) return;
        if (_isSpeedBoostActive) return;
        _isSpeedBoostActive = true;

        try
        {
            _fastRotationAnimation.From = (WheelIcon.RenderTransform as RotateTransform)?.Angle ?? 0;
            _fastRotationAnimation.To = _fastRotationAnimation.From + 360;
            WheelIcon.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, _fastRotationAnimation);

            await Task.Delay(3000);
            
            _defaultRotationAnimation.From = (WheelIcon.RenderTransform as RotateTransform)?.Angle ?? 0;
            _defaultRotationAnimation.To = _defaultRotationAnimation.From + 360;
            WheelIcon.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, _defaultRotationAnimation);
        }
        finally
        {
            _isSpeedBoostActive = false;
        }
    }
}
