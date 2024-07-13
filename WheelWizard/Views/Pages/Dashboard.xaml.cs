using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Utils;
using CT_MKWII_WPF.Utils.DolphinHelpers;
using MahApps.Metro.IconPacks;
using static CT_MKWII_WPF.Views.ViewUtils;
using Button = CT_MKWII_WPF.Views.Components.Button;

namespace CT_MKWII_WPF.Views.Pages;

public partial class Dashboard : Page
{
    public Dashboard()
    {
        InitializeComponent();
        UpdateActionButton();
    }

    private void GotoSettingsPage()
    {
        var layout = GetLayout();
        layout.SettingsButton.IsChecked = true;
        layout.NavigateToPage(new SettingsPage());
    }


    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        RRStatusManager.ActionButtonStatus status = await RRStatusManager.GetCurrentStatus();
        switch (status)
        {
            case RRStatusManager.ActionButtonStatus.NoServer: 
                GotoSettingsPage();
                break;
            case RRStatusManager.ActionButtonStatus.NoDolphin:
                GotoSettingsPage();
                break;
            case RRStatusManager.ActionButtonStatus.ConfigNotFinished:
                GotoSettingsPage();
                break;
            case RRStatusManager.ActionButtonStatus.noRR:
                SetButtonState("Installing...", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download, false, true);
                DisableSidebarButtons();
                await RetroRewindInstaller.InstallRetroRewind();
                EnableSidebarButtons();
                break;
            case RRStatusManager.ActionButtonStatus.OutOfDate:
                SetButtonState("Updating...", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Update, false, true);
                DisableSidebarButtons();
                await RetroRewindInstaller.UpdateRR();
                EnableSidebarButtons();
                break;
            case RRStatusManager.ActionButtonStatus.UpToDate:
                RetroRewindLauncher.PlayRetroRewind();
                break;
        }
        UpdateActionButton();
    }
    
    private async void UpdateActionButton()
    {
        RRStatusManager.ActionButtonStatus status = await RRStatusManager.GetCurrentStatus();
        switch (status)
        {
            case RRStatusManager.ActionButtonStatus.NoServer:
                SetButtonState("No Server", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.ServerNetworkOff);
                break;
            case RRStatusManager.ActionButtonStatus.NoDolphin:
                SetButtonState("Settings", Button.ButtonsVariantType.Secondary, PackIconFontAwesomeKind.FilePenSolid);
                break;
            case RRStatusManager.ActionButtonStatus.ConfigNotFinished:
                SetButtonState("Config Not Finished", Button.ButtonsVariantType.Secondary, PackIconFontAwesomeKind.FilePenSolid);
                break;
            case RRStatusManager.ActionButtonStatus.noRR:
                SetButtonState("Install", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download);
                break;
            case RRStatusManager.ActionButtonStatus.noRRActive:
                //this is here for future use,
                //right now there is no de-activation, but if we want multiple mods this might be handy
                SetButtonState("Activated", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Power);
                break;
            case RRStatusManager.ActionButtonStatus.RRnotReady:
                SetButtonState("Activate", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Power);
                break;
            case RRStatusManager.ActionButtonStatus.OutOfDate:
                SetButtonState("Update", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download);
                break;
            case RRStatusManager.ActionButtonStatus.UpToDate:
                SetButtonState("Play", Button.ButtonsVariantType.Primary, PackIconFontAwesomeKind.PlaySolid);
                break;
        }
    }
    
    private void SetButtonState(string text, Button.ButtonsVariantType variant, PackIconMaterialKind iconKind, bool enabled = true, bool subButtonsEnabled = true)
    {
        PlayButton.Text = text;
        PlayButton.Variant = variant;
        PlayButton.IsEnabled = enabled;
        PlayButton.IconPack = "Material";
        PlayButton.IconKind = iconKind;
        MiiButton.IsEnabled = subButtonsEnabled;
        DolphinButton.IsEnabled = subButtonsEnabled;
    }
    
    private void SetButtonState(string text, Button.ButtonsVariantType variant, PackIconFontAwesomeKind iconKind, bool enabled = true, bool subButtonsEnabled = true)
    {
        PlayButton.Text = text;
        PlayButton.Variant = variant;
        PlayButton.IsEnabled = enabled;
        PlayButton.IconPack = "FontAwesome";
        PlayButton.IconKind = iconKind;
        MiiButton.IsEnabled = subButtonsEnabled;
        DolphinButton.IsEnabled = subButtonsEnabled;
    }
    
    private void DisableSidebarButtons() => GetLayout().SidePanelButtons.IsEnabled = false;
    private void EnableSidebarButtons() => GetLayout().SidePanelButtons.IsEnabled = true;
    
    private void DolphinButton_OnClick(object sender, RoutedEventArgs e) => DolphinSettingHelper.LaunchDolphin();
    private void MiiButton_OnClick(object sender, RoutedEventArgs e)
    {
        WiiMoteSettings.EnableVirtualWiiMote();
        MiiChannelManager.LaunchMiiChannel();
    }
}
