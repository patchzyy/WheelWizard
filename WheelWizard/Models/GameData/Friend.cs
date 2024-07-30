using CT_MKWII_WPF.Services.WiiManagement;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Models.GameData;

public class Friend : INotifyPropertyChanged
{
    public string? Name { get; set; }
    public string? FriendCode { get; set; }
    public uint Wins { get; set; }
    public uint Losses { get; set; }
    public uint Vr { get; set; }
    public uint Br { get; set; }
    public string? MiiData { get; set; }

    private bool _requestingImage;
    private BitmapImage? _miiImage;
    public BitmapImage? MiiImage
    {
        get
        {
            if (_miiImage != null || _requestingImage) return _miiImage;

            _requestingImage = true;
            if (MiiData == null) return null;
            _miiImage = MiiImageManager.GetCachedMiiImage(MiiData);
            if (_miiImage == null)
            {
                MiiImageManager.LoadBase64MiiImageAsync(MiiData).ContinueWith(t =>
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
