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
    public required string FriendCode { get; set; }
    public required uint Vr { get; set; }
    public required uint Br { get; set; }
    
    public required uint RegionID { get; set; } 
    
    public string RegionName => Humanizer.GetRegionName(RegionID);
    

    public bool IsOnline
    {
        get
        {
            var currentRooms = RRLiveRooms.Instance.CurrentRooms;
            if (currentRooms.Count > 0)
            {
                var onlinePlayers = currentRooms.SelectMany(room => room.Players.Values).ToList();
                return onlinePlayers.Any(player => player.Fc == FriendCode);
            }
            return false;
        }
        set
        {
            if (value == IsOnline) return;
            OnPropertyChanged(nameof(IsOnline));
        }
    }

    public required MiiData MiiData { get; set; }
    
    private bool _isLoadingMiiImage;
    private BitmapImage? _miiImage;
    public BitmapImage? MiiImage
    {
        get
        {
            if (_miiImage != null) return _miiImage;
            if (!_isLoadingMiiImage)
            {
                LoadMiiImageAsync();
            }
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
        if (_isLoadingMiiImage || MiiData?.mii?.Data == null) return;

        _isLoadingMiiImage = true;
        try
        {
            var cachedImage = MiiImageManager.GetCachedMiiImage(MiiData.mii.Data);
            if (cachedImage != null)
            {
                MiiImage = cachedImage;
            }
            else
            {
                var loadedImage = await MiiImageManager.LoadBase64MiiImageAsync(MiiData.mii.Data);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MiiImage = loadedImage;
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
        get => MiiData.mii?.Name ?? "";
        set
        {
            if (MiiData == null)
            {
                MiiData = new MiiData
                {
                    mii = new Mii { Data = "", Name = value }
                };
            }
            else if (MiiData.mii == null)
            {
                MiiData.mii = new Mii { Data = "", Name = value };
            }
            else
            {
                MiiData.mii.Name = value;
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
