using System;
using System.Windows;
using System.Windows.Controls;
using WheelWizard.Models.Enums;
using WheelWizard.Models.Settings;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.Other;
using WheelWizard.Services.Settings;
using WheelWizard.Services.WiiManagement.SaveData;

namespace WheelWizard.WPFViews.Pages;

public partial class UserProfilePage : Page
{
    private int _currentUserIndex;
    private int FocussedUser => (int)SettingsManager.FOCUSSED_USER.Get();

    public UserProfilePage()
    {
        InitializeComponent();
        _currentUserIndex = FocussedUser;
        PopulateMii();
        FavoriteCheckBox.Checked += SetFavoriteUser;
        PopulateRegions();
        SetRegionDropdown();
    }

    private void SetRegionDropdown()
    {
        var region = (MarioKartWiiEnums.Regions)SettingsManager.RR_REGION.Get();
    
        foreach (var item in RegionDropdown.Items)
        {
            if (item is ComboBoxItem comboItem && 
                comboItem.Tag is MarioKartWiiEnums.Regions tagRegion && 
                tagRegion == region)
            {
                RegionDropdown.SelectedItem = comboItem;
                break;
            }
        }
        
        foreach (var item in RegionDropdownIfBroken.Items)
        {
            if (item is ComboBoxItem comboItem && 
                comboItem.Tag is MarioKartWiiEnums.Regions tagRegion && 
                tagRegion == region)
            {
                RegionDropdownIfBroken.SelectedItem = comboItem;
                break;
            }
        }
    }


    private void PopulateMii()
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
    
    private void PopulateRegions()
    {
        var validRegions = RRRegionManager.GetValidRegions();
        foreach (var region in Enum.GetValues<MarioKartWiiEnums.Regions>())
        {
            if (region == MarioKartWiiEnums.Regions.None)
                continue;
        
            // Create a new ComboBoxItem for the first dropdown
            var itemForRegionDropdown = new ComboBoxItem
            {
                Content = region.ToString(),
                Tag = region,
                IsEnabled = validRegions.Contains(region)
            };

            // Create a separate ComboBoxItem for the second dropdown
            var itemForRegionDropdownIfBroken = new ComboBoxItem
            {
                Content = region.ToString(),
                Tag = region,
                IsEnabled = validRegions.Contains(region)
            };

            RegionDropdown.Items.Add(itemForRegionDropdown);
            RegionDropdownIfBroken.Items.Add(itemForRegionDropdownIfBroken);
        }
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

    private void RegionDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RegionDropdown.SelectedItem is not ComboBoxItem item || 
            item.Tag is not MarioKartWiiEnums.Regions region) 
            return;
        RegionDropdownIfBroken.SelectedItem = RegionDropdown.SelectedItem;

        SettingsManager.RR_REGION.Set(region);
        GameDataLoader.Instance.LoadGameData();
        CurrentUserProfile.PopulateComponent();
        PopulateMii();
        SetRegionDropdown();
    }

    private void RegionDropdownIfBroken_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RegionDropdownIfBroken.SelectedItem is not ComboBoxItem item || 
            item.Tag is not MarioKartWiiEnums.Regions region) 
            return;

        SettingsManager.RR_REGION.Set(region);
        GameDataLoader.Instance.LoadGameData();
        CurrentUserProfile.PopulateComponent();
        PopulateMii();
        SetRegionDropdown();
    }
}
