using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WheelWizard.Helpers;
using WheelWizard.Models.Settings;
using WheelWizard.Resources.Languages;
using WheelWizard.Services;
using WheelWizard.Services.Settings;
using WheelWizard.Views.Popups.Generic;

namespace WheelWizard.Views.Pages.Settings;

public partial class WhWzSettings : UserControl
{
    private readonly bool _pageLoaded;
    private bool _editingScale;

    public WhWzSettings()
    {
        InitializeComponent();
        AutoFillUserPath();
        TogglePathSettings(false);
        LoadSettings();
        _pageLoaded = true;
    }

    private void LoadSettings()
    {
        // -----------------
        // Loading all the Window Scale settings
        // -----------------

        // IMPORTANT: Make sure that the number and percentage is always the last word in the string,
        // If you don want this, you should change the code below that parses the string back to an actual value
        var windowScaleOptions = new List<string>();
        foreach (var scale in SettingValues.WindowScales)
        {
            windowScaleOptions.Add(ScaleToString(scale));
        }

        var selectedItemText = ScaleToString((double)SettingsManager.WINDOW_SCALE.Get());
        if (!windowScaleOptions.Contains(selectedItemText))
            windowScaleOptions.Add(selectedItemText);
        
        

        EnableAnimations.IsChecked = (bool)SettingsManager.ENABLE_ANIMATIONS.Get();
    }

    private static string ScaleToString(double scale)
    {
        var percentageString = (int)Math.Round(scale * 100) + "%";
        if (SettingValues.WindowScales.Contains(scale))
            return percentageString;

        return "Custom: " + percentageString;
    }

    private void AutoFillUserPath()
    {
        if (DolphinExeInput.Text != "")
            return;

        var folderPath = PathManager.TryFindDolphinPath();
        if (!string.IsNullOrEmpty(folderPath))
            DolphinUserPathInput.Text = folderPath;
    }

    private async void DolphinExeBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var currentPath = (string)SettingsManager.DOLPHIN_LOCATION.Get();
        
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Dolphin Emulator",
            AllowMultiple = false,
            FileTypeFilter = new[] {
                new FilePickerFileType("Executable files")
                {
                    Patterns = new[] { "*.exe" }
                },
                new FilePickerFileType("All files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count >= 1)
        {
            DolphinExeInput.Text = files[0].Path.AbsolutePath;
        }
    }

    private async void GameLocationBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var currentPath = (string)SettingsManager.GAME_LOCATION.Get();
        
        var topLevel = TopLevel.GetTopLevel(this);
        
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Mario Kart Wii Game File",
            AllowMultiple = false,
            FileTypeFilter = new[] {
                new FilePickerFileType("Game files")
                {
                    Patterns = new[] { "*.iso", "*.wbfs", "*.rvz" }
                },
                new FilePickerFileType("All files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count >= 1)
        {
            MarioKartInput.Text = files[0].Path.AbsolutePath;
        }
    }

    private async void DolphinUserPathBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var currentFolder = (string)SettingsManager.USER_FOLDER_PATH.Get();
        
        var topLevel = TopLevel.GetTopLevel(this);
        
        if (!string.IsNullOrEmpty(currentFolder) && Directory.Exists(currentFolder))
        {
            var folder = await topLevel!.StorageProvider.TryGetFolderFromPathAsync(currentFolder);
            
            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                SuggestedStartLocation = folder,
                AllowMultiple = false
            });
            
            if (folders.Count >= 1)
                DolphinUserPathInput.Text = folders[0].Path.AbsolutePath;
        }
        else
        {
            var folderPath = PathManager.TryFindDolphinPath();
            if (!string.IsNullOrEmpty(folderPath))
            {
                // Ask user if they want to use the automatically found folder
                var result = new YesNoWindow()
                    .SetMainText($"{Phrases.PopupText_DolphinFoundText}\n{folderPath}")
                    .SetExtraText(Phrases.PopupText_DolphinFound).AwaitAnswer();
                if (result)
                {
                    DolphinUserPathInput.Text = folderPath;
                    return;
                }
            }
            else
            {
                new YesNoWindow().SetMainText(Phrases.PopupText_DolphinNotFoundText);
            }
            
            var folders = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                AllowMultiple = false
            });
            
            if (folders.Count >= 1)
                DolphinUserPathInput.Text = folders[0].Path.AbsolutePath;
        }
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
            new MessageBoxWindow().SetMainText(Phrases.PopupText_EnsurePathsExists).ShowDialog();
        }
        else
        {
            new MessageBoxWindow().SetMainText(Phrases.PopupText_SettingsSaved).ShowDialog();

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
        Process.Start("explorer.exe", PathManager.WheelWizardAppdataPath);
    }

    private void TogglePathSettings(bool enable)
    {
        if (!SettingsHelper.PathsSetupCorrectly() && !enable)
        {
            LocationBorder.BorderBrush = Avalonia.Media.Brushes.Orange;
            LocationEditButton.Variant = Components.Button.ButtonsVariantType.Warning;
            LocationWarningIcon.IsVisible = true;
        }
        else
        {
            LocationBorder.BorderBrush = Avalonia.Media.Brushes.Gray; // Use an appropriate color from your theme
            LocationEditButton.Variant = Components.Button.ButtonsVariantType.Primary;
            LocationWarningIcon.IsVisible = false;
        }

        LocationInputFields.IsEnabled = enable;
        LocationEditButton.IsVisible = !enable;
        LocationSaveButton.IsVisible = enable;
        LocationCancelButton.IsVisible = enable;

        DolphinExeInput.Text = PathManager.DolphinFilePath;
        MarioKartInput.Text = PathManager.GameFilePath;
        DolphinUserPathInput.Text = PathManager.UserFolderPath;
    }
    

    private void EnableAnimations_OnClick(object sender, RoutedEventArgs e) =>
        SettingsManager.ENABLE_ANIMATIONS.Set(EnableAnimations.IsChecked == true);
}
