using CT_MKWII_WPF.Models.Enums;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using CT_MKWII_WPF.Services.Other;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement;
using MahApps.Metro.IconPacks;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static CT_MKWII_WPF.Views.ViewUtils;
using Button = CT_MKWII_WPF.Views.Components.Button;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CT_MKWII_WPF.Views.Pages;

public partial class Dashboard
{
    public Dashboard()
    {
        InitializeComponent();
        UpdateActionButton();
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
                NavigateToPage(new Settings.SettingsPage());
                break;
            case WheelWizardStatus.ConfigNotFinished:
                NavigateToPage(new Settings.SettingsPage());
                break;
            case WheelWizardStatus.NoRR:
                SetButtonState(Common.Installing_State, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download,
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
                SetButtonState(Common.Updating_State, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Update, false);
                DisableSidebarButtons();
                await RetroRewindUpdater.UpdateRR();
                EnableSidebarButtons();
                break;
            case WheelWizardStatus.UpToDate:
                await Launcher.LaunchRetroRewind();
                break;
        }
        UpdateActionButton();
        DisableAllButtonsTemporarily();
    }

    private async void UpdateActionButton()
    {
        SetButtonState(Common.Loading_State, Button.ButtonsVariantType.Secondary, 
                       PackIconFontAwesomeKind.SpinnerSolid, false);
        _status = await StatusManager.GetCurrentStatus();
        switch (_status)
        {
            case WheelWizardStatus.NoServer:
                SetButtonState(Common.NoServer, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.ServerNetworkOff);
                break;
            case WheelWizardStatus.NoServerButInstalled:
                SetButtonState(Common.PlayOffline, Button.ButtonsVariantType.Secondary, PackIconFontAwesomeKind.PlaySolid);
                break;
            case WheelWizardStatus.NoDolphin:
                SetButtonState("Settings", Button.ButtonsVariantType.Secondary, PackIconFontAwesomeKind.FilePenSolid);
                break;
            case WheelWizardStatus.ConfigNotFinished:
                SetButtonState(Common.ConfigNotFinished, Button.ButtonsVariantType.Secondary,
                    PackIconFontAwesomeKind.FilePenSolid);
                break;
            case WheelWizardStatus.NoRR:
                SetButtonState(Common.Install, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download);
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
                SetButtonState(Common.Update, Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download);
                break;
            case WheelWizardStatus.UpToDate:
                SetButtonState(Common.Play, Button.ButtonsVariantType.Primary, PackIconFontAwesomeKind.PlaySolid);
                break;
        }
        if (SettingsHelper.PathsSetupCorrectly()) 
            return;
        
        DolphinButton.IsEnabled = false;
    }

    private void SetButtonState(string text, Button.ButtonsVariantType variant, PackIconMaterialKind iconKind,
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

    private void SetButtonState(string text, Button.ButtonsVariantType variant, PackIconFontAwesomeKind iconKind,
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
}
