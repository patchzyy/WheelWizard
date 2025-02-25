﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WheelWizard.Models.Enums;
using WheelWizard.Models.GameData;
using WheelWizard.Models.RRInfo;
using WheelWizard.Models.Settings;
using WheelWizard.Resources.Languages;
using WheelWizard.Services.LiveData;
using WheelWizard.Utilities;
using WheelWizard.WPFViews.Pages;

namespace WheelWizard.WPFViews.Components;

public sealed partial class PlayerStatsComponent : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isOnline;
    public bool IsOnline
    {
        get => _isOnline;
        set => SetProperty(ref _isOnline, value);
    }
    
    private string _playerName;
    public string PlayerName
    {
        get => _playerName;
        set
        {
            if(value == SettingValues.NoName)
                SetProperty(ref _playerName, Online.NoName);
            if(value == SettingValues.NoLicense)
                SetProperty(ref _playerName, Online.NoLicense);
            else 
                SetProperty(ref _playerName, value);
        }
    }

    private string _friendCode;
    public string FriendCode
    {
        get => _friendCode;
        set => SetProperty(ref _friendCode, value);
    }

    private string _vr;
    public string VR
    {
        get => _vr;
        set => SetProperty(ref _vr, value);
    }

    private string _br;
    public string BR
    {
        get => _br;
        set => SetProperty(ref _br, value);
    }
    
    private string _topExtraStat;
    public string TopExtraStat
    {
        get => _topExtraStat;
        set => SetProperty(ref _topExtraStat, value);
    }
    
    private string _bottomExtraStat;
    public string BottomExtraStat
    {
        get => _bottomExtraStat;
        set => SetProperty(ref _bottomExtraStat, value);
    }
    
    private String _regionName;
    public String RegionName
    {
        get => _regionName;
        set => SetProperty(ref _regionName, value);
    }

    private string _onlineText;
    public string OnlineText
    {
        get => _onlineText;
        set => SetProperty(ref _onlineText, value);
    }
    
    private Mii? _mii;
    public Mii? Mii
    {
        get => _mii;
        set => SetProperty(ref _mii, value);
    }
    
    private PlayerWinPosition? _winPosition;
    public PlayerWinPosition? WinPosition
    {
        get => _winPosition;
        set => SetProperty(ref _winPosition, value);
    }
    
    public PlayerStatsComponent() 
    { 
        InitializeComponent();
        DataContext = this;
    }
    
    public void UpdateStats(User user)
    {
        PlayerName = user.MiiName;
        if (PlayerName == SettingValues.NoName)
            PlayerName = Online.NoName;
        if (PlayerName == SettingValues.NoLicense)
            PlayerName = Online.NoLicense;
        
        FriendCode = user.FriendCode;
        VR = "VR: " + user.Vr;
        BR = "BR: " + user.Br;
        BottomExtraStat = $"{Online.Stat_RacesPlayed}: {user.TotalRaceCount}";
        TopExtraStat = $"{Online.Stat_Wins}: {user.TotalWinCount}";
        WinPosition = FriendCodeManager.GetWinPosition(FriendCode);
        Mii = user?.MiiData?.Mii;
        IsOnline = user!.IsOnline;
        ViewRoomButton.Visibility = IsOnline ? Visibility.Visible : Visibility.Hidden;
        OnlineText = IsOnline ? Common.Term_Online : Common.Term_Offline;
        RegionName = user.RegionName;
    }

    public void UpdateStats(Friend friend)
    {
        PlayerName = friend.MiiName;
        if (PlayerName == SettingValues.NoName)
            PlayerName = Online.NoName;
        if (PlayerName == SettingValues.NoLicense)
            PlayerName = Online.NoLicense;
        
        FriendCode = friend.FriendCode;
        VR = "VR: " + friend.Vr;
        BR = "BR: " + friend.Br;
        BottomExtraStat = $"{Online.Stat_Wins}: {friend.Wins}";
        TopExtraStat = $"{Online.Stat_Losses}: {friend.Losses}";
        Mii = friend?.MiiData?.Mii;
        IsOnline = friend!.IsOnline;
        OnlineText = IsOnline ? Common.Term_Online : Common.Term_Offline;
        ViewRoomButton.Visibility = IsOnline ? Visibility.Visible : Visibility.Hidden;
        RegionName = friend.CountryName;
    }
    
    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;
        
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ViewButton_OnClick(object sender, RoutedEventArgs e)
    {
        var allRooms = RRLiveRooms.Instance.CurrentRooms;
        foreach (var room in allRooms)
        {
            if (room.Players.All(player => player.Value.Fc != FriendCode))
                continue;

            var currentPage = Window.GetWindow(this) as Layout;        
            currentPage?.NavigateToPage(new RoomDetailPage(room));
            return;
        }
    }

    private void CopyFriendCode_OnClick(object sender, RoutedEventArgs e)
    {
        IDataObject dataObject = new DataObject();
        dataObject.SetData(DataFormats.Text, FriendCode);
        Clipboard.SetDataObject(dataObject);
    }
}
