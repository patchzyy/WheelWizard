using CT_MKWII_WPF.Services.WiiManagement;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using CT_MKWII_WPF.Utilities.RepeatedTasks;
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
    public partial class CurrentUserProfileComponent : UserControl, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private ImageSource _miiImage;
        public ImageSource MiiImage
        {
            get => _miiImage;
            set
            {
                _miiImage = value;
                OnPropertyChanged(nameof(MiiImage));
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
        
        private DispatcherTimer _refreshTimer;

        public CurrentUserProfileComponent()
        {
            InitializeComponent();
            DataContext = this;
            PopulateComponent();
            
            // Set up the refresh timer
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(6);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }
        
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            PopulateComponent();
        }
        public async void PopulateComponent()
        {
            GameDataLoader.Instance.RefreshOnlineStatus();
            var currentUser = GameDataLoader.Instance.GetCurrentUser;
            PlayerName = currentUser.MiiName;
            FriendCode = currentUser.FriendCode;
            VrAndBr = "VR: " + currentUser.Vr;
            MiiImage = await MiiImageManager.LoadBase64MiiImageAsync(currentUser.MiiData.mii.Data);
            IsOnline = currentUser.IsOnline;
        }

        private void Profile_click(object sender, MouseButtonEventArgs e)
        {
            if (!GameDataLoader.Instance.HasAnyValidUsers)
            {
                return;
            }
            var currentPage = (Layout)Window.GetWindow(this);
            currentPage?.NavigateToPage(new UserProfilePage());
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public void Dispose()
        {
            _refreshTimer.Stop();
            _refreshTimer = null;
        }
    }
}
