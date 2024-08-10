using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.LiveData;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using CT_MKWII_WPF.Services.WiiManagement;
using System.Linq;
using System.Windows;

namespace CT_MKWII_WPF.Models.GameData;

public abstract class BasePlayer : INotifyPropertyChanged
{
    public required string FriendCode { get; init; }
    public required uint Vr { get; init; }
    public required uint Br { get; init; }
    public required uint RegionId { get; init; } 
    
    public string RegionName => Humanizer.GetRegionName(RegionId);
    
    public bool IsOnline
    {
        get
        {
            var currentRooms = RRLiveRooms.Instance.CurrentRooms;
            if (currentRooms.Count <= 0) return false;

            var onlinePlayers = currentRooms.SelectMany(room => room.Players.Values).ToList();
            return onlinePlayers.Any(player => player.Fc == FriendCode);
        }
        set
        {
            if (value == IsOnline) return;
            
            OnPropertyChanged(nameof(IsOnline));
        }
    }

    public required MiiData? MiiData { get; set; }
    
    private bool _isLoadingMiiImage;
    private BitmapImage? _miiImage;
    public BitmapImage? MiiImage
    {
        get
        {
            if (_miiImage != null) return _miiImage;
            if (!_isLoadingMiiImage)
                LoadMiiImageAsync();
            return null;
        }
        set
        {
            if (_miiImage == value) return;
            
            _miiImage = value;
            OnPropertyChanged(nameof(MiiImage));
        }
    }
    
    private async void LoadMiiImageAsync()
    {
        if (_isLoadingMiiImage || MiiData?.Mii?.Data == null) return;

        _isLoadingMiiImage = true;
        try
        {
            var cachedImage = MiiImageManager.GetCachedMiiImage(MiiData.Mii.Data);
            if (cachedImage != null)
                MiiImage = new BitmapImage();
            else
            {
                var loadedImage = await MiiImageManager.LoadBase64MiiImageAsync(MiiData.Mii.Data);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MiiImage = new BitmapImage();
                });
            }
        }
        finally
        {
            _isLoadingMiiImage = false;
        }
    }


    public string MiiName
    {
        get => MiiData?.Mii?.Name ?? "";
        set
        {
            if (MiiData == null)
            {
                MiiData = new MiiData
                {
                    Mii = new Mii { Data = "", Name = value }
                };
            }
            else if (MiiData.Mii == null)
                MiiData.Mii = new Mii { Data = "", Name = value };
            else
                MiiData.Mii.Name = value;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
