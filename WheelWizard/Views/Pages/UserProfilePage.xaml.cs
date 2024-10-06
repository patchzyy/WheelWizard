using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement.SaveData;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages;

public partial class UserProfilePage : Page
{
    private int _currentUserIndex;
    private int FocussedUser => (int)SettingsManager.FOCUSSED_USER.Get();

    public UserProfilePage()
    {
        InitializeComponent();
  
        _currentUserIndex = FocussedUser;
        PopulateMiiOnStartup();
        FavoriteCheckBox.Checked += SetFavoriteUser;
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
        FavoriteCheckBox.IsChecked = FocussedUser == _currentUserIndex;
    }
    
    private void SetFavoriteUser(object sender, RoutedEventArgs e)
    {
        SettingsManager.FOCUSSED_USER.Set(_currentUserIndex);
        GameDataLoader.Instance.LoadGameData();
        GameDataLoader.Instance.RefreshOnlineStatus();
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

            
            var miiName = data.Users[i].MiiData?.Mii?.Name ?? SettingValues.NoName;
            var noLicense = miiName == SettingValues.NoLicense;
            
            radioButton.Content = miiName;
            radioButton.IsEnabled = !noLicense;
            if (noLicense)
                radioButton.FontStyle = FontStyles.Italic;
            
            if (miiName == SettingValues.NoName) 
                radioButton.Content = Online.NoName;
            if (miiName == SettingValues.NoLicense) 
                radioButton.Content = Online.NoLicense;
        }
    }

    private void SetInitialSelectedMii()
    {
        if (FindName($"Mii{_currentUserIndex}") is RadioButton radioButton)
            radioButton.IsChecked = true;
    }
}
