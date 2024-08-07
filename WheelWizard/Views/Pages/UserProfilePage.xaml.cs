using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages;

public partial class UserProfilePage : Page
{
    private int _currentUserIndex;

    public UserProfilePage()
    {
        InitializeComponent();
        _currentUserIndex = ConfigManager.GetConfig().FavoriteUser;
        PopulateMiiNames();
        PopulateMiiOnStartup();
        FavoriteCheckBox.Checked += SetFavoriteUser;
        FavoriteCheckBox.Unchecked += UnsetFavoriteUser;
    }

    private void TopBarRadio_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton button && int.TryParse(button.Tag.ToString().Replace("Mii", ""), out int userIndex))
        {
            _currentUserIndex = userIndex - 1; // Adjust for 0-based index
            UpdatePlayerStats();
            UpdateFavoriteCheckBox();
        }
    }
    
    private void SetFavoriteUser(object sender, RoutedEventArgs e)
    {
        var config = ConfigManager.GetConfig();
        config.FavoriteUser = _currentUserIndex;
        ConfigManager.SaveConfigToJson();
        GameDataLoader.Instance.LoadGameData();
        GameDataLoader.Instance.RefreshOnlineStatus();
        CurrentUserProfile.PopulateComponent();

    }

    private void UnsetFavoriteUser(object sender, RoutedEventArgs e)
    {
        var config = ConfigManager.GetConfig();
        if (config.FavoriteUser == _currentUserIndex)
        {
            config.FavoriteUser = 0; // 0 is backup
            ConfigManager.SaveConfigToJson();
            GameDataLoader.Instance.LoadGameData();
            CurrentUserProfile.PopulateComponent();
        }
    }
    
    private void PopulateMiiOnStartup()
    {
        UpdatePlayerStats();
        UpdateFavoriteCheckBox();
        SetInitialSelectedMii();
    }

    private void PopulateMiiNames()
    {
        var data = GameDataLoader.Instance.GetGameData;
        var userAmount = data.Users.Count;
        for (int i = 0; i < userAmount; i++)
        {
            var radioButton = (RadioButton)FindName($"Mii{i + 1}");
            if (radioButton != null!)
            {
                if(data.Users[i].MiiData.mii.Name == "No License")
                {
                    radioButton.Content = data.Users[i].MiiData.mii.Name;
                    radioButton.IsEnabled = false;
                    radioButton.FontStyle = FontStyles.Italic;
                    continue;
                }
                radioButton.Content = data.Users[i].MiiData.mii.Name;
                radioButton.IsEnabled = true;
            }
        }
    }

    private void UpdatePlayerStats()
    {
        PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(_currentUserIndex));
    }

    private void UpdateFavoriteCheckBox()
    {
        FavoriteCheckBox.IsChecked = ConfigManager.GetConfig().FavoriteUser == _currentUserIndex;
    }

    private void SetInitialSelectedMii()
    {
        var radioButton = (RadioButton)FindName($"Mii{_currentUserIndex + 1}");
        if (radioButton != null)
        {
            radioButton.IsChecked = true;
        }
    }
}
