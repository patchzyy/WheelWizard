using CT_MKWII_WPF.Models.RRInfo;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using CT_MKWII_WPF.Services.WiiManagement;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Models.GameData;

public abstract class BasePlayer : INotifyPropertyChanged
{
    public required string FriendCode { get; set; }
    public required uint Vr { get; set; }
    public required uint Br { get; set; }
    public required bool IsOnline { get; set; }
    public required GameDataLoader.MiiData MiiData { get; set; }

    private bool _requestingImage;
    private BitmapImage? _miiImage;
    public BitmapImage? MiiImage
    {
        get
        {
            if (_miiImage != null || _requestingImage) return _miiImage;

            _requestingImage = true;
            if (MiiData.mii.Data == null) return null;
            _miiImage = MiiImageManager.GetCachedMiiImage(MiiData.mii.Data);
            if (_miiImage == null)
            {
                MiiImageManager.LoadBase64MiiImageAsync(MiiData.mii.Data).ContinueWith(t =>
                {
                    _miiImage = t.Result;
                    _requestingImage = false;
                    OnPropertyChanged(nameof(MiiImage));
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            return _miiImage;
        }
        set
        {
            if (_miiImage == value) return;
            _miiImage = value;
            OnPropertyChanged(nameof(MiiImage));
        }
    }
    public string MiiName
    {
        get => MiiData?.mii?.Name ?? "";
        set
        {
            if (MiiData == null)
            {
                MiiData = new GameDataLoader.MiiData
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
