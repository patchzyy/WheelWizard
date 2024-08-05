using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages;

public partial class UserProfilePage : Page
{
    private int _currentUserIndex = GameDataLoader.Instance.getGameData.CurrentUserIndex;
    public UserProfilePage()
    {
        InitializeComponent();
        PopulateMiiNames();
        populateMiiOnStartup();
    }

    private void TopBarRadio_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;
        var tag = button.Tag.ToString();
        FavoriteCheckBox.IsChecked = false;
        switch (tag)
        {
            case "Mii1":
                PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(0));
                if (ConfigManager.GetConfig().FavoriteUser == 0)
                {
                    FavoriteCheckBox.IsChecked = true;
                }
                break;
            case "Mii2":
                PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(1));
                if (ConfigManager.GetConfig().FavoriteUser == 1)
                {
                    FavoriteCheckBox.IsChecked = true;
                }
                break;
            case "Mii3":
                PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(2));
                if (ConfigManager.GetConfig().FavoriteUser == 2)
                {
                    FavoriteCheckBox.IsChecked = true;
                }
                break;
            case "Mii4":
                PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(3));
                if (ConfigManager.GetConfig().FavoriteUser == 3)
                {
                    FavoriteCheckBox.IsChecked = true;
                }
                break;
            default:
                break;
        }
    }
    
    private async void populateMiiOnStartup()
    {
        PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(_currentUserIndex));
    }

    private void PopulateMiiNames()
    {
        var data = GameDataLoader.Instance.getGameData;
        Mii1.Content = data.Users[0].MiiData.mii.Name;
        Mii2.Content = data.Users[1].MiiData.mii.Name;
        Mii3.Content = data.Users[2].MiiData.mii.Name;
        Mii4.Content = data.Users[3].MiiData.mii.Name;
    }
}
