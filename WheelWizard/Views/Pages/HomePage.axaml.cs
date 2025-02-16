using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WheelWizard.Models.Enums;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.Installation;
using WheelWizard.Services.Launcher;
using WheelWizard.Services.Launcher.Helpers;
using WheelWizard.Services.Settings;
using WheelWizard.Views.Pages.Settings;
using Button = WheelWizard.Views.Components.StandardLibrary.Button;

namespace WheelWizard.Views.Pages;

public partial class HomePage : UserControl
{
    private static ILauncher currentLauncher => _launcherTypes[_launcherIndex];
    private static int _launcherIndex = 0; // Make sure this index never goes over the list index
    private static List<ILauncher> _launcherTypes = new List<ILauncher>()
    { // Add Launcher instances here , every launch instance is a new start screen
        RetroRewindLauncher.Instance
    };

    private WheelWizardStatus _status;
    private MainButtonState currentButtonState => buttonStates[_status];
    private static Dictionary<WheelWizardStatus, MainButtonState> buttonStates =
        new Dictionary<WheelWizardStatus, MainButtonState>()
        {
            {
                WheelWizardStatus.Loading,
                new(Common.State_Loading, Button.ButtonsVariantType.Default, "Spinner", null, false)
            },
            {
                WheelWizardStatus.NoServer,
                new(Common.State_NoServer, Button.ButtonsVariantType.Danger, "RoadError", null, true)
            },
            {
                WheelWizardStatus.NoServerButInstalled,
                new(Common.Action_PlayOffline, Button.ButtonsVariantType.Warning, "Play", LaunchGame, true)
            },
            {
                WheelWizardStatus.NoDolphin,
                new("Dolphin not setup", Button.ButtonsVariantType.Warning, "Settings", NavigateToSettings, false)
            },
            {
                WheelWizardStatus.ConfigNotFinished,
                new(Common.State_ConfigNotFinished, Button.ButtonsVariantType.Warning, "Settings", NavigateToSettings, true)
            },
            {
                WheelWizardStatus.NotDownloaded,
                new(Common.Action_Install, Button.ButtonsVariantType.Warning, "Download", Download, true)
            },
            {
                WheelWizardStatus.OutOfDate,
                new(Common.Action_Update, Button.ButtonsVariantType.Warning, "Download", Update, true)
            },
            {
                WheelWizardStatus.Ready,
                new(Common.Action_Play, Button.ButtonsVariantType.Primary, "Play", LaunchGame, true)
            }
        };
    

    public HomePage()
    {
        InitializeComponent();
        UpdatePage();
    }

    private void UpdatePage()
    {
        GameTitle.Text = currentLauncher.GameTitle;
        UpdateActionButton();
    }

    private void DolphinButton_OnClick(object? sender, RoutedEventArgs e)
    {
        DolphinLaunchHelper.LaunchDolphin();
        DisableAllButtonsTemporarily();
    }

    private static void LaunchGame() => currentLauncher.Launch();
    private static void NavigateToSettings() => ViewUtils.NavigateToPage(new SettingsPage());

    private async static void Download()
    {
        ViewUtils.GetLayout().DisableEverything();
        await currentLauncher.Install();
        ViewUtils.GetLayout().EnableEverything();
    }

    private async static void Update()
    {
        ViewUtils.GetLayout().DisableEverything();
        await currentLauncher.Update();
        ViewUtils.GetLayout().EnableEverything();
    }
    
    private void PlayButton_Click(object? sender, RoutedEventArgs e)
    {
        currentButtonState?.OnClick?.Invoke();
        
        UpdateActionButton();
        DisableAllButtonsTemporarily();
    }

    private async void UpdateActionButton()
    {
        _status = WheelWizardStatus.Loading;
        SetButtonState(currentButtonState); 
        _status = await currentLauncher.GetCurrentStatus();
        SetButtonState(currentButtonState); 
    }

    private void SetButtonState(MainButtonState state)
    {
        PlayButton.Text = state.Text;
        PlayButton.Variant = state.Type;
        PlayButton.IsEnabled = state.OnClick != null;
        if (Application.Current != null && Application.Current.FindResource(state.IconName) is Geometry geometry)
                PlayButton.IconData = geometry;
        DolphinButton.IsEnabled = state.SubButtonsEnabled && SettingsHelper.PathsSetupCorrectly();
    }
    private void DisableAllButtonsTemporarily()
    {
        CompleteGrid.IsEnabled = false;
        //wait 5 seconds before re-enabling the buttons
        Task.Delay(5000).ContinueWith(_ =>
        {
            Dispatcher.UIThread.InvokeAsync(() => CompleteGrid.IsEnabled = true);
        });
    }
    
    public class MainButtonState
    {
        public MainButtonState(string text, Button.ButtonsVariantType type, string iconName, Action? onClick, bool subButtonsEnables) => 
        (Text, Type, IconName, OnClick, SubButtonsEnabled) = (text, type, iconName, onClick, subButtonsEnables);
        
        public string Text { get; set; }
        public Button.ButtonsVariantType Type { get; set; }
        public string IconName { get; set; }
        public Action? OnClick { get; set; }
        public bool SubButtonsEnabled { get; set; }
    }
}
