using CT_MKWII_WPF.Models.Enums;
using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Resources.Languages;
using CT_MKWII_WPF.Services.WiiManagement.SaveData;
using CT_MKWII_WPF.Views.Pages;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
                _playerName = value == SettingValues.NoName ? Online.NoName : value;
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
        
        
        private PlayerWinPosition? _winPosition;
        public PlayerWinPosition? WinPosition
        {
            get => _winPosition;
            set
            {
                _winPosition = value;
                OnPropertyChanged(nameof(WinPosition));
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
            
            if (PlayerName == SettingValues.NoName)
                PlayerName = Online.NoName;
            if (PlayerName == SettingValues.NoLicense)
                PlayerName = Online.NoLicense;
            
            FriendCode = currentUser.FriendCode;
            VrAndBr = "VR: " + currentUser.Vr;
            Mii = currentUser.Mii;
            IsOnline = currentUser.IsOnline;
        }
        
        private void Profile_click(object sender, RoutedEventArgs e) => ViewUtils.NavigateToPage(new UserProfilePage());

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
