using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.IdkWhereThisShouldGo;
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
        LoadSettings();
        FillUserPath();
        UpdateResolutionButtonsState();
        ToggleLocationSettings(false);
        VersionText.Text = "v" + AutoUpdater.CurrentVersion;
    }

    private void LoadSettings()
    {
        if (!ConfigValidator.ConfigCorrectAndExists()) return;
        DolphinExeInput.Text = PathManager.GetDolphinLocation();
        MarioKartInput.Text = PathManager.GetGameLocation();
        DolphinUserPathInput.Text = PathManager.GetUserPathLocation();
    }

    private void UpdateResolution(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton) return;
        var resolution = radioButton.Tag.ToString();
        DolphinSettingHelper.ChangeIniSettings(PathManager.FindGfxFile(), "Settings", "InternalResolution", resolution);
    }

    private void UpdateResolutionButtonsState()
    {
        var enableControls = ConfigValidator.ConfigCorrectAndExists();
        VideoBorder.IsEnabled = enableControls;
        WiiBorder.IsEnabled = enableControls;

        if (!enableControls) return;
        VSyncButton.IsChecked = DolphinSettingsUtils.GetCurrentVSyncStatus();
        RecommendedButton.IsChecked = DolphinSettingsUtils.IsRecommendedSettingsEnabled();
        var finalResolution = 0;
        var resolution =
            DolphinSettingHelper.ReadIniSetting(PathManager.FindGfxFile(), "Settings", "InternalResolution");
        if (int.TryParse(resolution, out var parsedResolution))
        {
            finalResolution = parsedResolution - 1;
        }

        if (finalResolution >= 0 && finalResolution < ResolutionStackPanel.Children.Count)
        {
            var radioButton = (RadioButton)ResolutionStackPanel.Children[finalResolution];
            radioButton.IsChecked = true;
        }

        var forcemote = WiiMoteSettings.IsForceSettingsEnabled();
        DisableForce.IsChecked = forcemote;
    }

    private void FillUserPath()
    {
        if (DolphinExeInput.Text != "") return;
        var folderPath = DolphinSettingHelper.AutomaticallyFindDolphinPath();
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
        var folderPath = DolphinSettingHelper.AutomaticallyFindDolphinPath();

        if (!string.IsNullOrEmpty(folderPath))
        {
            // Ask user if they want to use the automatically found folder
            var result = MessageBox.Show(
                "**If you dont know what all of this means, just click yes :)**\n\nDolphin Emulator folder found. Would you like to use this folder?\n\n" +
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

    private void VSync_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO: Move this when the dolphin settings stuff has been updated
        DolphinSettingHelper.ChangeIniSettings(PathManager.FindGfxFile(), "Hardware", "VSync",
            VSyncButton.IsChecked == true ? "True" : "False");
    }

    private void Recommended_OnClick(object sender, RoutedEventArgs e)
    {
        if (RecommendedButton.IsChecked == true)
            DolphinSettingsUtils.EnableRecommendedSettings();
        else
            DolphinSettingsUtils.DisableRecommendedSettings();
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dolphinPath = DolphinExeInput.Text;
        var gamePath = MarioKartInput.Text;
        var userFolder = DolphinUserPathInput.Text;
        var forceDisableWiimote = DisableForce.IsChecked == true;
        if (!File.Exists(dolphinPath) || !File.Exists(gamePath) || !Directory.Exists(userFolder))
        {
            MessageBox.Show("Please ensure all paths are correct and try again.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        // Save settings to a configuration file or system registry
        ConfigManager.SaveSettings(dolphinPath, gamePath, userFolder, true, forceDisableWiimote);
        if (!ConfigValidator.SetupCorrectly())
        {
            MessageBox.Show("Please ensure all paths are correct and try again.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            NavigateToPage(new SettingsPage());
            return;
        }

        UpdateResolutionButtonsState();
        ToggleLocationSettings(false);
        MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void EditButton_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleLocationSettings(true);
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleLocationSettings(false);
        LoadSettings();
    }

    private void ToggleLocationSettings(bool enable)
    {
        if (!ConfigValidator.ConfigCorrectAndExists() && !enable)
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
    }

    private void ClickForceWiimote(object sender, RoutedEventArgs e)
    {
        ConfigValidator.SaveWiimoteSettings(DisableForce.IsChecked == true);
    }
}
