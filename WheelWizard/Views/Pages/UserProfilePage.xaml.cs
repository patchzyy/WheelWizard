using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement.SaveData;
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
        PopulateMiiOnStartup();
        FavoriteCheckBox.Checked += SetFavoriteUser;
        
        // TODO: unchecked is not needed anymore when we make it a radio button
        //       we also want to make the styles for checkbox and radio buttons universal that
        //       they can be used interchangeably
        FavoriteCheckBox.Unchecked += UnsetFavoriteUser;
    }

        
    private void PopulateMiiOnStartup()
    {
        var validUsers = GameDataLoader.Instance.HasAnyValidUsers;
        
        VisibleWithProfiles.Visibility = validUsers ? Visibility.Visible : Visibility.Collapsed;
        VisibleWithoutProfiles.Visibility = validUsers ? Visibility.Collapsed : Visibility.Visible;
        RadioButtons.Visibility = validUsers ? Visibility.Visible : Visibility.Collapsed;
        PageTitle.VerticalAlignment = validUsers ? VerticalAlignment.Top : VerticalAlignment.Bottom;
        
        if (!validUsers) 
            return;
        
        PopulateMiiNames();
        SetInitialSelectedMii();
        UpdatePage();
    }
    
    private void TopBarRadio_OnClick(object sender, RoutedEventArgs e)
    {
        var oldIndex = _currentUserIndex;
        if (sender is not RadioButton button ||
            !int.TryParse((string?)button.Tag, out _currentUserIndex )) 
            return;
        
        if (oldIndex == _currentUserIndex) 
            return;
        
        UpdatePage();
    }
    
    private void UpdatePage()
    {
        PlayerStats.UpdateStats(GameDataLoader.Instance.GetUserData(_currentUserIndex));
        FavoriteCheckBox.IsChecked = ConfigManager.GetConfig().FavoriteUser == _currentUserIndex;
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
        if (config.FavoriteUser != _currentUserIndex) 
            return;

        config.FavoriteUser = 0; // 0 is backup
        ConfigManager.SaveConfigToJson();
        GameDataLoader.Instance.LoadGameData();
        CurrentUserProfile.PopulateComponent();
    }

    private void PopulateMiiNames()
    {
        var data = GameDataLoader.Instance.GetGameData;
        var userAmount = data.Users.Count;
        for (var i = 0; i < userAmount; i++)
        {
            var radioButton = FindName($"Mii{i}") as RadioButton;
            if (radioButton == null!) 
                continue;

            var miiName = data.Users[i].MiiData?.Mii?.Name ?? "No Name";
            var noLicense = miiName == "No License";
            
            radioButton.Content = miiName;
            radioButton.IsEnabled = !noLicense;
            if(noLicense)
                radioButton.FontStyle = FontStyles.Italic;
        }
    }

    private void SetInitialSelectedMii()
    {
        if (FindName($"Mii{_currentUserIndex}") is RadioButton radioButton)
            radioButton.IsChecked = true;
    }
}
