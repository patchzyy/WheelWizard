using CT_MKWII_WPF.Services.WiiManagement.GameData;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages;

public partial class UserProfilePage : Page
{
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

        switch (tag)
        {
            case "Mii1":
                PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(0));
                break;
            case "Mii2":
                PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(1));
                break;
            case "Mii3":
                PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(2));
                break;
            case "Mii4":
                PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(3));
                break;
            default:
                break;
        }
    }
    
    private async void populateMiiOnStartup()
    {
        var users = GameDataLoader.Instance.GetAllUsers;
        foreach (var user in users)
        {
            if (user.MiiData.mii.Name == "" || user.MiiData.mii.Name == null) continue;
            PlayerStats.UpdateStats(user);
            break;
        }
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
