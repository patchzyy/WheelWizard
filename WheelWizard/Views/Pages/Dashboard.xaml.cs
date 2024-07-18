using System.Windows;
using MahApps.Metro.IconPacks;
using System.Threading.Tasks;
using CT_MKWII_WPF.Models.Enums;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Launcher;
using CT_MKWII_WPF.Services.Networking;
using CT_MKWII_WPF.Services.Validators;
using CT_MKWII_WPF.Services.WiiManagement;
using CT_MKWII_WPF.Services.WiiManagement.DolphinHelpers;
using static CT_MKWII_WPF.Views.ViewUtils;
using Button = CT_MKWII_WPF.Views.Components.Button;

namespace CT_MKWII_WPF.Views.Pages;

public partial class Dashboard
{
    public Dashboard()
    {
        InitializeComponent();
        UpdateActionButton();
    }

    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        ActionButtonStatus status = await RRStatusManager.GetCurrentStatus();
        switch (status)
        {
            case ActionButtonStatus.NoServer:
                NavigateToPage(new SettingsPage());
                break;
            case ActionButtonStatus.NoDolphin:
                NavigateToPage(new SettingsPage());
                break;
            case ActionButtonStatus.ConfigNotFinished:
                NavigateToPage(new SettingsPage());
                break;
            case ActionButtonStatus.NoRR:
                SetButtonState("Installing...", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download,
                    false);
                DisableSidebarButtons();
                await RetroRewindInstaller.InstallRetroRewind();
                EnableSidebarButtons();
                break;
            case ActionButtonStatus.OutOfDate:
                SetButtonState("Updating...", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Update, false);
                DisableSidebarButtons();
                await RRUpdater.UpdateRR();
                EnableSidebarButtons();
                break;
            case ActionButtonStatus.UpToDate:
                RetroRewindLauncher.PlayRetroRewind((bool)OnlineTTCheckbox.IsChecked!);
                break;
        }

        UpdateActionButton();
        DisableAllButtonsTemporarily();
    }

    private async void UpdateActionButton()
    {
        ActionButtonStatus status = await RRStatusManager.GetCurrentStatus();
        switch (status)
        {
            case ActionButtonStatus.NoServer:
                SetButtonState("No Server", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.ServerNetworkOff);
                break;
            case ActionButtonStatus.NoDolphin:
                SetButtonState("Settings", Button.ButtonsVariantType.Secondary, PackIconFontAwesomeKind.FilePenSolid);
                break;
            case ActionButtonStatus.ConfigNotFinished:
                SetButtonState("Config Not Finished", Button.ButtonsVariantType.Secondary,
                    PackIconFontAwesomeKind.FilePenSolid);
                break;
            case ActionButtonStatus.NoRR:
                SetButtonState("Install", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download);
                break;
            case ActionButtonStatus.NoRRActive:
                //this is here for future use,
                //right now there is no de-activation, but if we want multiple mods this might be handy
                SetButtonState("Activated", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Power);
                break;
            case ActionButtonStatus.RRNotReady:
                SetButtonState("Activate", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Power);
                break;
            case ActionButtonStatus.OutOfDate:
                SetButtonState("Update", Button.ButtonsVariantType.Secondary, PackIconMaterialKind.Download);
                break;
            case ActionButtonStatus.UpToDate:
                SetButtonState("Play", Button.ButtonsVariantType.Primary, PackIconFontAwesomeKind.PlaySolid);
                break;
        }

        if (!ConfigValidator.ConfigCorrectAndExists())
        {
            DolphinButton.IsEnabled = false;
            MiiButton.IsEnabled = false;
            OnlineTTCheckbox.IsEnabled = false;
        }
    }

    private void SetButtonState(string text, Button.ButtonsVariantType variant, PackIconMaterialKind iconKind,
        bool enabled = true, bool subButtonsEnabled = true)
    {
        PlayButton.Text = text;
        PlayButton.Variant = variant;
        PlayButton.IsEnabled = enabled;
        PlayButton.IconPack = "Material";
        PlayButton.IconKind = iconKind;
        MiiButton.IsEnabled = subButtonsEnabled;
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
        MiiButton.IsEnabled = subButtonsEnabled;
        DolphinButton.IsEnabled = subButtonsEnabled;
    }

    private void DisableSidebarButtons() => GetLayout().SidePanelButtons.IsEnabled = false;
    private void EnableSidebarButtons() => GetLayout().SidePanelButtons.IsEnabled = true;

    private void DolphinButton_OnClick(object sender, RoutedEventArgs e)
    {
        DolphinSettingHelper.LaunchDolphin();
        DisableAllButtonsTemporarily();
    }

    private void MiiButton_OnClick(object sender, RoutedEventArgs e)
    {
        WiiMoteSettings.EnableVirtualWiiMote();
        _ = MiiChannelManager.LaunchMiiChannel();
        DisableAllButtonsTemporarily();
    }

    private void DisableAllButtonsTemporarily()
    {
        CompleteGrid.IsEnabled = false;
        //wait 5 seconds before re-enabling the buttons
        Task.Delay(5000).ContinueWith(_ => { Dispatcher.Invoke(() => CompleteGrid.IsEnabled = true); });
    }
}