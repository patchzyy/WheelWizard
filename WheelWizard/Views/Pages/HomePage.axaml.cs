using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using WheelWizard.Models.Enums;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.Installation;
using WheelWizard.Services.Launcher;
using WheelWizard.Services.Other;
using WheelWizard.Services.Settings;
using WheelWizard.Views.Components;
using WheelWizard.Views.Pages.Settings;
using Button = WheelWizard.Views.Components.Button;

namespace WheelWizard.Views.Pages;

public partial class HomePage : UserControl
{
    private double _wheelRotationAngle = 0;
    private WheelWizardStatus _status;
    public HomePage()
    {
        InitializeComponent();
        UpdateActionButton();
        // StartWheelRotation(); // Removed spinning for now
    }

    private async void StartWheelRotation()
    {
        // while (true) // Infinite loop for continuous rotation
        // {
        //     _wheelRotationAngle = (_wheelRotationAngle + 0.1) % 360; // Increment the angle, reset at 360
        //     // WheelRotation.Angle = _wheelRotationAngle; // Update the rotation angle
        //     await Task.Delay(10); // A small delay to control the speed of rotation
        // }
    }
    

    private void DolphinButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Launcher.LaunchDolphin();
        DisableAllButtonsTemporarily();
    }

    private async void PlayButton_Click(object? sender, RoutedEventArgs e)
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
                ViewUtils.NavigateToPage(new SettingsPage());
                break;
            case WheelWizardStatus.ConfigNotFinished:
                ViewUtils.NavigateToPage(new SettingsPage());
                break;
            case WheelWizardStatus.NoRR:
                SetButtonState(Common.State_Installing, Button.ButtonsVariantType.Warning, "Download",
                    false);
                DisableSidebarButtons();
                await RetroRewindInstaller.InstallRetroRewind();
                EnableSidebarButtons();
                break;
            case WheelWizardStatus.RRNotReady:
                SetButtonState("Activating...", Button.ButtonsVariantType.Warning, "RoadWarning", false);
                DisableSidebarButtons();
                await RetroRewindInstaller.InstallRetroRewind();
                EnableSidebarButtons();
                break;
            case WheelWizardStatus.OutOfDate:
                SetButtonState(Common.State_Updating, Button.ButtonsVariantType.Warning, "Download", false);
                DisableSidebarButtons();
                await RetroRewindUpdater.UpdateRR();
                EnableSidebarButtons();
                break;
            case WheelWizardStatus.UpToDate:
                // SpeedBoostWheel(); // Removed spinning for now
                await Launcher.LaunchRetroRewind();
                break;
        }
        UpdateActionButton();
        DisableAllButtonsTemporarily();
    }

    private async void UpdateActionButton()
    {
        SetButtonState(Common.State_Loading, Button.ButtonsVariantType.Warning, 
            "Spinner", false); 
        _status = await StatusManager.GetCurrentStatus();
        switch (_status)
        {
            case WheelWizardStatus.NoServer:
                SetButtonState(Common.State_NoServer, Button.ButtonsVariantType.Warning, "RoadError");
                break;
            case WheelWizardStatus.NoServerButInstalled:
                SetButtonState(Common.Action_PlayOffline, Button.ButtonsVariantType.Warning, "Play");
                break;
            case WheelWizardStatus.NoDolphin:
                SetButtonState(Common.PageTitle_Settings, Button.ButtonsVariantType.Warning, "Settings");
                break;
            case WheelWizardStatus.ConfigNotFinished:
                SetButtonState(Common.State_ConfigNotFinished, Button.ButtonsVariantType.Warning, "Settings");
                break;
            case WheelWizardStatus.NoRR:
                SetButtonState(Common.Action_Install, Button.ButtonsVariantType.Warning, "Download");
                break;
            case WheelWizardStatus.NoRRActive:
                SetButtonState("Activated", Button.ButtonsVariantType.Warning, "RoadWarning"); // Check if this  is still relevant
                break;
            case WheelWizardStatus.RRNotReady:
                SetButtonState("Activate", Button.ButtonsVariantType.Warning, "RoadWarning"); // Check if this  is still relevant
                break;
            case WheelWizardStatus.OutOfDate:
                SetButtonState(Common.Action_Update, Button.ButtonsVariantType.Warning, "Download");
                break;
            case WheelWizardStatus.UpToDate:
                SetButtonState(Common.Action_Play, Button.ButtonsVariantType.Primary, "Play");
                break;
        }
        if (SettingsHelper.PathsSetupCorrectly()) 
            return;
     
        DolphinButton.IsEnabled = false;
    }

    private void SetButtonState(string text, Components.Button.ButtonsVariantType variant, string iconKey,
        bool enabled = true, bool subButtonsEnabled = true)
    {
        PlayButton.Text = text;
        PlayButton.Variant = variant;
        PlayButton.IsEnabled = enabled;
        if (Application.Current != null && Application.Current.FindResource(iconKey) is Geometry geometry)
                PlayButton.IconData = geometry;
        DolphinButton.IsEnabled = subButtonsEnabled && SettingsHelper.PathsSetupCorrectly();
    }
    
    private void DisableSidebarButtons() => Layout.Instance.SidePanelButtons.IsEnabled = false;
    private void EnableSidebarButtons() => Layout.Instance.SidePanelButtons.IsEnabled = true;

    private void DisableAllButtonsTemporarily()
    {
        CompleteGrid.IsEnabled = false;
        //wait 5 seconds before re-enabling the buttons
        Task.Delay(5000).ContinueWith(_ =>
        {
            Dispatcher.UIThread.InvokeAsync(() => CompleteGrid.IsEnabled = true);
        });
    }
}
