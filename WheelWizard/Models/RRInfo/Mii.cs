using CT_MKWII_WPF.Services.WiiManagement;
using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Models.RRInfo;

public class Mii : INotifyPropertyChanged
{
    public required string Data { get; set; }
    public required string Name { get; set; }
    public bool LoadedImageSuccessfully { get; private set; } // default false, dont set this manually
    
    // This never will be set back to false, this is intentional
    // This is to ensure that it will never request the image again after the first time
    private bool _requestingImage;
    private BitmapImage? _image;

    public BitmapImage? Image
    {
        get
        {
            if (_image != null || _requestingImage) return _image;
            // it will set it to true, meaning this code can only be executed once due to the above check
            _requestingImage = true; 
            
            var newImage = MiiImageManager.GetCachedMiiImage(Data);
            if (newImage == null)
                MiiImageManager.ResetMiiImageAsync(this);
            else
                SetImage(newImage.Value.Item1, newImage.Value.Item2);
            
            return _image;
        }
        private set
        {
            if (_image == value) return;
            _image = value;
            OnPropertyChanged(nameof(Image));
        }
    }
    
    public void SetImage(BitmapImage image, bool loadedSuccessfully)
    {
        LoadedImageSuccessfully = loadedSuccessfully;
        Image = image;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
