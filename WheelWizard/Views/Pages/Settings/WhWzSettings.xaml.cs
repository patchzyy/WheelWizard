using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.Settings;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Button = CT_MKWII_WPF.Views.Components.Button;


namespace CT_MKWII_WPF.Views.Pages.Settings;

public partial class WhWzSettings : UserControl
{
    public WhWzSettings()
    {
        InitializeComponent();
        AutoFillUserPath();
        TogglePathSettings(false);
        LoadSettings();
    }

    private void LoadSettings()
    {
        
        // -----------------
        // Loading all the Window Scale settings
        // -----------------
        
        // IMPORTANT: Make sure that the number and percentage is always the last word in the string,
        // If you don want this, you should change the code below that parses the string back to an actual value
        foreach(double scale in SettingValues.WindowScales)
        {
            var scaleText = (int)Math.Round(scale * 100) + "%";
            WindowScaleDropdown.Items.Add(scaleText);
        }
        var selectedItemText = (int)Math.Round((double)SettingsManager.WINDOW_SCALE.Get() * 100) + "%";
        // check if the selected item is in the list
        if (WindowScaleDropdown.Items.Contains(selectedItemText))
            WindowScaleDropdown.SelectedItem = selectedItemText;
        else
        {
            // This will most users never see. This will only be visible if you change the WindowScale value in the json itself
            // to a value that is not listed in the WindowScales array. It is mandatory however, since we dont want
            // To break the app just because some users have a different scale. (also if we change the dropdown values in the future)
            selectedItemText = "Custom: " + selectedItemText;
            WindowScaleDropdown.Items.Add(selectedItemText);
            WindowScaleDropdown.SelectedItem = selectedItemText;
        }
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
        var currentPath = (string)SettingsManager.DOLPHIN_LOCATION.Get();
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
            Title = "Select Dolphin Emulator"
        };
        
        if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(Path.GetDirectoryName(currentPath)))
            openFileDialog.InitialDirectory = Path.GetDirectoryName(currentPath)!;
    
        if (openFileDialog.ShowDialog() == true)
            DolphinExeInput.Text = openFileDialog.FileName;
    }

    private void GameLocationBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var currentPath = (string)SettingsManager.GAME_LOCATION.Get();
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Game files (*.iso;*.wbfs;*.rvz)|*.iso;*.wbfs;*.rvz|All files (*.*)|*.*",
            Title = "Select Mario Kart Wii Game File"
        };
    
        if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(Path.GetDirectoryName(currentPath)))
            openFileDialog.InitialDirectory = Path.GetDirectoryName(currentPath)!;
        
        if (openFileDialog.ShowDialog() == true)
            MarioKartInput.Text = openFileDialog.FileName;
    }

    private void DolphinUserPathBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var currentFolder = (string)SettingsManager.USER_FOLDER_PATH.Get();
        CommonOpenFileDialog dialog = new();
        dialog.IsFolderPicker = true;
        if (!string.IsNullOrEmpty(currentFolder) && Directory.Exists(currentFolder))
            dialog.InitialDirectory = currentFolder;
        else
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
        }
        
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            DolphinUserPathInput.Text = dialog.FileName;
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var oldPath1 = (string)SettingsManager.DOLPHIN_LOCATION.Get();
        var oldPath2 = (string)SettingsManager.GAME_LOCATION.Get();
        var oldPath3 = (string)SettingsManager.USER_FOLDER_PATH.Get();
        
        var path1 = SettingsManager.DOLPHIN_LOCATION.Set(DolphinExeInput.Text);
        var path2 = SettingsManager.GAME_LOCATION.Set(MarioKartInput.Text);
        var path3 = SettingsManager.USER_FOLDER_PATH.Set(DolphinUserPathInput.Text);
        // These 3 lines is only saving the settings
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
            
            // This is not really the best approach, but it works for now
            if (oldPath1 + oldPath2 + oldPath3 != DolphinExeInput.Text + MarioKartInput.Text + DolphinUserPathInput.Text)
                DolphinSettingManager.Instance.ReloadSettings();
        }
    }
    
    private void CancelButton_OnClick(object sender, RoutedEventArgs e) => TogglePathSettings(false);
    private void EditButton_OnClick(object sender, RoutedEventArgs e) => TogglePathSettings(true);

    private void Folder_Click(object sender, RoutedEventArgs e)
    {
        if (!Directory.Exists(PathManager.WheelWizardAppdataPath))
            Directory.CreateDirectory(PathManager.WheelWizardAppdataPath);
        Process.Start("explorer.exe",PathManager.WheelWizardAppdataPath);
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
        
        DolphinExeInput.Text = PathManager.DolphinFilePath;
        MarioKartInput.Text = PathManager.GameFilePath;
        DolphinUserPathInput.Text = PathManager.UserFolderPath;
    }
    
    private void WindowScaleDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedLanguage = WindowScaleDropdown.SelectedItem.ToString();
        var scale = double.Parse(selectedLanguage.Split(" ").Last()
                                                 .Replace("%", "")) / 100;
   
        SettingsManager.WINDOW_SCALE.Set(scale);
    }
}
