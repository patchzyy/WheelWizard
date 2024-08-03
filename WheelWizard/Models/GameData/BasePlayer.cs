using System.ComponentModel;
using System.Windows.Media.Imaging;
using CT_MKWII_WPF.Services.WiiManagement;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Models.GameData;

public abstract class BasePlayer : INotifyPropertyChanged
{
    public required string FriendCode { get; set; }
    public required uint Vr { get; set; }
    public required uint Br { get; set; }
    public required bool IsOnline { get; set; }

    protected string? MiiBinaryData { get; set; }

    private bool _requestingImage;
    private BitmapImage? _miiImage;
    public BitmapImage? MiiImage
    {
        get
        {
            if (_miiImage != null || _requestingImage) return _miiImage;

            _requestingImage = true;
            if (MiiBinaryData == null) return null;
            _miiImage = MiiImageManager.GetCachedMiiImage(MiiBinaryData);
            if (_miiImage == null)
            {
                MiiImageManager.LoadBase64MiiImageAsync(MiiBinaryData).ContinueWith(t =>
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
