﻿using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.WiiManagement;
using CT_MKWII_WPF.Services.WiiManagement.SaveData;
using CT_MKWII_WPF.Views.Pages;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CT_MKWII_WPF.Views.Components
{
    public sealed partial class CurrentUserProfileComponent : UserControl, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _playerName;
        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged(nameof(PlayerName));
            }
        }

        private string _friendCode;
        public string FriendCode
        {
            get => _friendCode;
            set
            {
                _friendCode = value;
                OnPropertyChanged(nameof(FriendCode));
            }
        }

        private string _vrAndBr;
        public string VrAndBr
        {
            get => _vrAndBr;
            set
            {
                _vrAndBr = value;
                OnPropertyChanged(nameof(VrAndBr));
            }
        }

        private Mii? _mii;
        public Mii? Mii
        {
            get => _mii;
            set
            {
                _mii = value;
                OnPropertyChanged(nameof(Mii));
            }
        }
        
        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            set
            {
                _isOnline = value;
                OnPropertyChanged(nameof(IsOnline));
            }
        }
        
        private DispatcherTimer? _refreshTimer;

        public CurrentUserProfileComponent()
        {
            InitializeComponent();
            DataContext = this;
            PopulateComponent();
            
            // Set up the refresh timer
            _refreshTimer = new() { Interval = TimeSpan.FromSeconds(60) };
            _refreshTimer.Tick += RefreshTimer_Tick!;
            _refreshTimer.Start();
        }
        
        private void RefreshTimer_Tick(object sender, EventArgs e) => PopulateComponent();
        
        public async void PopulateComponent()
        {
            GameDataLoader.Instance.RefreshOnlineStatus();
            var currentUser = GameDataLoader.Instance.GetCurrentUser;
            PlayerName = currentUser.MiiName;
            FriendCode = currentUser.FriendCode;
            VrAndBr = "VR: " + currentUser.Vr;
            Mii = currentUser.MiiData.Mii;
            IsOnline = currentUser.IsOnline;
        }
        
        private void Profile_click(object sender, RoutedEventArgs e)
        {
            if (!GameDataLoader.Instance.HasAnyValidUsers)
                return;
 
            var currentPage = Window.GetWindow(this) as Layout;
            currentPage?.NavigateToPage(new UserProfilePage());
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public void Dispose()
        {
            _refreshTimer?.Stop();
            _refreshTimer = null;
        }
    }
}
