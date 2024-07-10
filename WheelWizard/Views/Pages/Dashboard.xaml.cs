using System;
using System.Windows;
using System.Windows.Controls;
using CT_MKWII_WPF.Utils;
using CT_MKWII_WPF.Views.Components;
using MaterialDesignThemes.Wpf;

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
        var layout = (Layout) Application.Current.MainWindow;
        layout.SettingsButton.IsChecked = true;
        layout.NavigateToPage(new SettingsPage());
    }


    private async void PlayButtonClick(object sender, RoutedEventArgs e)
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
                SetButtonState("Installing...", "Secondary", false, PackIconKind.Download);
                DisableSidebarButtons();
                await RetroRewindInstaller.InstallRetroRewind();
                EnableSidebarButtons();
                break;
            case RRStatusManager.ActionButtonStatus.OutOfDate:
                SetButtonState("Updating...", "Secondary", false, PackIconKind.Update);
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
                SetButtonState("No Server", "Secondary", true, PackIconKind.ServerNetworkOff);
                break;
            case RRStatusManager.ActionButtonStatus.NoDolphin:
                SetButtonState("Settings", "Secondary", true, PackIconKind.Cog);
                break;
            case RRStatusManager.ActionButtonStatus.ConfigNotFinished:
                SetButtonState("Config Not Finished", "Secondary", true, PackIconKind.FileDocumentEdit);
                break;
            case RRStatusManager.ActionButtonStatus.noRR:
                SetButtonState("Install", "Secondary", true, PackIconKind.Download);
                break;
            case RRStatusManager.ActionButtonStatus.noRRActive:
                //this is here for future use,
                //right now there is no de-activation, but if we want multiple mods this might be handy
                SetButtonState("Activated", "Secondary", true, PackIconKind.Power);
                break;
            case RRStatusManager.ActionButtonStatus.RRnotReady:
                SetButtonState("Activate", "Secondary", true, PackIconKind.Power);
                break;
            case RRStatusManager.ActionButtonStatus.OutOfDate:
                SetButtonState("Update", "Secondary", true, PackIconKind.Download);
                break;
            case RRStatusManager.ActionButtonStatus.UpToDate:
                SetButtonState("Play", "Primary", true, PackIconKind.Play);
                break;
        }
    }
    
    private void SetButtonState(string text, string variant, bool isEnabled, PackIconKind iconKind)
    {
        ActionButton.Text = text;
        ActionButton.Variant = variant;
        ActionButton.IsEnabled = isEnabled;
        ActionButton.IconPack = "Material";
        ActionButton.IconKind = iconKind;
        ActionButton.IconKind = ActionButton.IconKind.ToString();
    }

    private void DisableSidebarButtons()
    {
        //reference to the current layout
        var layout = (Layout) Application.Current.MainWindow;
        layout.SidePanelButtons.IsEnabled = false;
    }
    
    private void EnableSidebarButtons()
    {
        //reference to the current layout
        var layout = (Layout) Application.Current.MainWindow;
        layout.SidePanelButtons.IsEnabled = true;
    }
    
}
