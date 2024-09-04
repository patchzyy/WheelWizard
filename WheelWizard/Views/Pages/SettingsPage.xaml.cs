using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static CT_MKWII_WPF.Views.ViewUtils;
using Button = CT_MKWII_WPF.Views.Components.Button;

namespace CT_MKWII_WPF.Views.Pages;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        AutoFillUserPath();
        UpdateSettingsState();
        TogglePathSettings(false);
            
        DisableForce.IsChecked = (bool)SettingsManager.FORCE_WIIMOTE.Get();
        LaunchWithDolphin.IsChecked = (bool)SettingsManager.LAUNCH_WITH_DOLPHIN.Get();
        
        WhWzVersionText.Text = "WhWz: v" + AutoUpdater.CurrentVersion;
        RrVersionText.Text = "RR: " + RetroRewindInstaller.CurrentRRVersion();
    }

    private void LoadPathSettings()
    {
        DolphinExeInput.Text = PathManager.DolphinFilePath;
        MarioKartInput.Text = PathManager.GameFilePath;
        DolphinUserPathInput.Text = PathManager.UserFolderPath;
    }

    private void UpdateResolution(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton) 
            return;
        
        SettingsManager.INTERNAL_RESOLUTION.Set(int.Parse(radioButton.Tag.ToString()!));
    }

    private void UpdateSettingsState()
    {
        var enableDolphinSettings = SettingsHelper.PathsSetupCorrectly();
        VideoBorder.IsEnabled = enableDolphinSettings;
        WiiBorder.IsEnabled = enableDolphinSettings;

        if (!enableDolphinSettings) 
            return;
        
        VSyncButton.IsChecked = (bool)SettingsManager.VSYNC.Get();
        RecommendedButton.IsChecked = (bool)SettingsManager.RECOMMENDED_SETTINGS.Get();
        Button30FPS.IsChecked = (bool)SettingsManager.FORCE_30FPS.Get();
        ShowFPSButton.IsChecked = (bool)SettingsManager.SHOW_FPS.Get();
        var finalResolution = (int)SettingsManager.INTERNAL_RESOLUTION.Get() - 1;
        if (finalResolution < 0 || finalResolution >= ResolutionStackPanel.Children.Count) 
            return;

        var radioButton = (RadioButton)ResolutionStackPanel.Children[finalResolution];
        radioButton.IsChecked = true;
    }

    private void AutoFillUserPath()
    {
        if (DolphinExeInput.Text != "") 
            return;
        
        var folderPath = PathManager.TryFindDolphinPath();
        if (!string.IsNullOrEmpty(folderPath))
            DolphinUserPathInput.Text = folderPath;
    }


    private void DolphinExeBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
            Title = "Select Dolphin Emulator"
        };

        if (openFileDialog.ShowDialog() == true)
            DolphinExeInput.Text = openFileDialog.FileName;
    }

    private void MarioKartBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Game files (*.iso;*.wbfs;*.rvz)|*.iso;*.wbfs;*.rvz|All files (*.*)|*.*",
            Title = "Select Mario Kart Wii Game File"
        };

        if (openFileDialog.ShowDialog() == true)
            MarioKartInput.Text = openFileDialog.FileName;
    }

    private void DolphinUserPathBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var folderPath = PathManager.TryFindDolphinPath();

        if (!string.IsNullOrEmpty(folderPath))
        {
            // Ask user if they want to use the automatically found folder
            var result = MessageBox.Show(
                "If you dont know what all of this means, just click yes :)\n\nDolphin Emulator folder found. Would you like to use this folder?\n" +
                folderPath, "Dolphin Emulator Folder Found", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DolphinUserPathInput.Text = folderPath;
                return;
            }
        }
        else
        {
            MessageBox.Show(
                "Dolphin Emulator folder not automatically found. Please try and find the folder manually, click 'help' for more information.",
                "Dolphin Emulator Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // If we end up here, this means either the path wasn't found, or the user wants to manually select the path
        // Make it so we have to select a folder, not an executable
        CommonOpenFileDialog dialog = new();
        dialog.IsFolderPicker = true;
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            DolphinUserPathInput.Text = dialog.FileName;
    }

    private void VSync_OnClick(object sender, RoutedEventArgs e) => SettingsManager.VSYNC.Set(VSyncButton.IsChecked == true);
    private void Recommended_OnClick(object sender, RoutedEventArgs e) => SettingsManager.RECOMMENDED_SETTINGS.Set(RecommendedButton.IsChecked == true);

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var path1 = SettingsManager.DOLPHIN_LOCATION.Set(DolphinExeInput.Text);
        var path2 = SettingsManager.GAME_LOCATION.Set(MarioKartInput.Text);
        var path3 = SettingsManager.USER_FOLDER_PATH.Set(DolphinUserPathInput.Text);
        // These 3 lines is only saving the settings
        
        // The rest is all visual feedback
        UpdateSettingsState();
        TogglePathSettings(false);
        if (!(SettingsHelper.PathsSetupCorrectly() && path1 && path2 && path3))
        {
            MessageBox.Show("Please ensure all paths are correct and try again.", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            MessageBox.Show("Settings saved successfully!", 
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void EditButton_OnClick(object sender, RoutedEventArgs e)
    {
        TogglePathSettings(true);
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        TogglePathSettings(false);
    }

    private void TogglePathSettings(bool enable)
    {
        if (!SettingsHelper.PathsSetupCorrectly() && !enable)
        {
            LocationBorder.BorderBrush = (SolidColorBrush)FindResource("SettingsBlockWarningHighlight");
            LocationEditButton.Variant = Button.ButtonsVariantType.Secondary;
            LocationWarningIcon.Visibility = Visibility.Visible;
        }
        else
        {
            LocationBorder.BorderBrush = (SolidColorBrush)FindResource("SettingsBlockBackground");
            LocationEditButton.Variant = Button.ButtonsVariantType.Primary;
            LocationWarningIcon.Visibility = Visibility.Collapsed;
        }

        LocationInputFields.IsEnabled = enable;
        LocationEditButton.Visibility = enable ? Visibility.Collapsed : Visibility.Visible;
        LocationSaveButton.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
        LocationCancelButton.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
        LoadPathSettings();
    }

    private void ClickForceWiimote(object sender, RoutedEventArgs e)
    {
        SettingsManager.FORCE_WIIMOTE.Set(DisableForce.IsChecked == true);
    }

    private void ClickLaunchWithDolphinWindow(object sender, RoutedEventArgs e)
    {
        SettingsManager.LAUNCH_WITH_DOLPHIN.Set(LaunchWithDolphin.IsChecked == true);
    }

    private void _30FPS_OnClick(object sender, RoutedEventArgs e)
    {
        SettingsManager.FORCE_30FPS.Set(Button30FPS.IsChecked == true);
    }

    private void ShowFPS_OnClick(object sender, RoutedEventArgs e)
    {
        SettingsManager.SHOW_FPS.Set(ShowFPSButton.IsChecked == true);
    }
}
