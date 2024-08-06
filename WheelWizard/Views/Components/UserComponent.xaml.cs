using CT_MKWII_WPF.Models.GameData;
using CT_MKWII_WPF.Services.WiiManagement;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using CT_MKWII_WPF.Utilities;
using CT_MKWII_WPF.Views.Pages;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class CurrentUserProfileComponent : UserControl, INotifyPropertyChanged
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

        public CurrentUserProfileComponent()
        {
            InitializeComponent();
            DataContext = this;
            PopulateComponent();
        }
        public async void PopulateComponent()
        {
            GameDataLoader.Instance.RefreshOnlineStatus();
            PlayerName = GameDataLoader.Instance.getCurrentUsername;
            FriendCode = GameDataLoader.Instance.getCurrentFriendCode;
            VrAndBr = "VR: " + GameDataLoader.Instance.getCurrentVr;
            MiiImage = await MiiImageManager.LoadBase64MiiImageAsync(GameDataLoader.Instance.getCurrentUser.MiiData.mii.Data);
            IsOnline = GameDataLoader.Instance.isCurrentUserOnline;
        }

        private void Profile_click(object sender, MouseButtonEventArgs e)
        {
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
    }
}
