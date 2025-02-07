using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System;
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
        // If you don't want this, you should change the code below that parses the string back to an actual value
      
        foreach (var scale in SettingValues.WindowScales)
        {
            WindowScaleDropdown.Items.Add(ScaleToString(scale));
        }

        var selectedItemText = ScaleToString((double)SettingsManager.WINDOW_SCALE.Get());
        if (!WindowScaleDropdown.Items.Contains(selectedItemText)) 
              WindowScaleDropdown.Items.Add(selectedItemText);
        WindowScaleDropdown.SelectedItem = selectedItemText;
        
        
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
        // Define platform-specific executable patterns
        var executableFileType = new FilePickerFileType("Executable files")
        {
            Patterns = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => new[] { "*.exe" },
                PlatformID.Unix or PlatformID.MacOSX => new[] { "*", "*.sh" }, // Unix systems often don't use extensions for executables
                _ => new[] { "*" } // Fallback
            }
        };

        var filePath = await FilePickerHelper.OpenSingleFileAsync("Select Dolphin Emulator", new[] { executableFileType });
        if (!string.IsNullOrEmpty(filePath))
        {
            DolphinExeInput.Text = filePath;
        }
    }

    private async void GameLocationBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var fileType = new FilePickerFileType("Game files")
        {
            Patterns = new[] { "*.iso", "*.wbfs", "*.rvz" }
        };

        var filePath = await FilePickerHelper.OpenSingleFileAsync("Select Mario Kart Wii Game File", new[] { fileType });
        if (!string.IsNullOrEmpty(filePath))
        {
            MarioKartInput.Text = filePath;
        }
    }

    private async void DolphinUserPathBrowse_OnClick(object sender, RoutedEventArgs e)
    {
        var currentFolder = (string)SettingsManager.USER_FOLDER_PATH.Get();
        var topLevel = TopLevel.GetTopLevel(this);

        // If a current folder exists and is valid, suggest it as the starting location
        if (!string.IsNullOrEmpty(currentFolder) && Directory.Exists(currentFolder))
        {
            var folder = await topLevel!.StorageProvider.TryGetFolderFromPathAsync(currentFolder);
            var folders = await FilePickerHelper.SelectFolderAsync("Select Dolphin User Path", folder);

            if (folders.Count >= 1)
                DolphinUserPathInput.Text = folders[0].Path.LocalPath;
            return;
        }

        // Attempt to find Dolphin's default path if no valid folder is set
        var folderPath = PathManager.TryFindDolphinPath();
        if (!string.IsNullOrEmpty(folderPath))
        {
            // Ask the user if they want to use the automatically found folder
            var result = await new YesNoWindow()
                .SetMainText($"{Phrases.PopupText_DolphinFoundText}\n{folderPath}")
                .SetExtraText(Phrases.PopupText_DolphinFound)
                .AwaitAnswer();

            if (result)
            {
                DolphinUserPathInput.Text = folderPath;
                return;
            }
        }
        else
        {
            // Notify the user that Dolphin could not be found
            await new MessageBoxWindow()
                .SetMainText(Phrases.PopupText_DolphinNotFoundText)
                .ShowDialog();
        }

        // Let the user manually select a folder
        var manualFolders = await FilePickerHelper.SelectFolderAsync("Select Dolphin User Path");

        if (manualFolders.Count >= 1)
            DolphinUserPathInput.Text = manualFolders[0].Path.LocalPath;
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
        
        FilePickerHelper.OpenFolderInFileManager(PathManager.WheelWizardAppdataPath);
    }

    private void TogglePathSettings(bool enable)
    {
        if (!SettingsHelper.PathsSetupCorrectly() && !enable)
        {
            var color = (Color)Application.Current.FindResource("Warning600");
            LocationBorder.BorderBrush = new SolidColorBrush(color);
            LocationEditButton.Variant = Components.StandardLibrary.Button.ButtonsVariantType.Warning;
            LocationWarningIcon.IsVisible = true;
        }
        else
        {
            var color = (Color)Application.Current.FindResource("Primary600");
            LocationBorder.BorderBrush = new SolidColorBrush(color);
            LocationEditButton.Variant = Components.StandardLibrary.Button.ButtonsVariantType.Primary;
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
    
    private async void WindowScaleDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_pageLoaded || _editingScale)
            return;
        
        _editingScale = true;
        var selectedLanguage = WindowScaleDropdown.SelectedItem.ToString();
        var scale = double.Parse(selectedLanguage!.Split(" ").Last()
            .Replace("%", "")) / 100;
   
        SettingsManager.WINDOW_SCALE.Set(scale);
        var seconds = 10;
        string ExtraText() => $"This change will revert in {Humanizer.HumanizeSeconds(seconds)} " 
                              + $"unless you decide to keep the change.";
        
        var yesNoWindow = new YesNoWindow()
            .SetButtonText(Common.Action_Apply, Common.Action_Revert)
            .SetMainText(Phrases.PopupText_ApplyScale)
            .SetExtraText(ExtraText());
        // we want to now set up a timer every second to update the text, and at the last second close the window
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
       
        timer.Tick += (_, args) =>
        {
            seconds--;
            yesNoWindow.SetExtraText(ExtraText());
            if (seconds != 0) 
                return;
            yesNoWindow.Close();
            timer.Stop();
        };
        timer.Start();

        var yesNoAnswer = await yesNoWindow.AwaitAnswer();
        if (yesNoAnswer)
            SettingsManager.SAVED_WINDOW_SCALE.Set(SettingsManager.WINDOW_SCALE.Get());
        else
        {
            SettingsManager.WINDOW_SCALE.Set(SettingsManager.SAVED_WINDOW_SCALE.Get());
            WindowScaleDropdown.SelectedItem = ScaleToString((double)SettingsManager.WINDOW_SCALE.Get());
        }
        
        _editingScale = false;
    }
    
    private void EnableAnimations_OnClick(object sender, RoutedEventArgs e) =>
        SettingsManager.ENABLE_ANIMATIONS.Set(EnableAnimations.IsChecked == true);
}
