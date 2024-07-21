using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Models.RRInfo;

public class Player : INotifyPropertyChanged
{
    public required string Count { get; set; } // you can have one Wii that with 2 players (and hence the Mii list)
    public required string Pid { get; set; }
    public required string Name { get; set; }
    public required string ConnMap { get; set; } // its always there, but we dont use 
    public required string ConnFail { get; set; }
    public required string Suspend { get; set; }
    public required string Fc { get; set; }
    public required string Ev { get; set; } = "--"; // private games don't have EV and EB
    public required string Eb { get; set; }  = "--";
    public required List<Mii> Mii { get; set; } = new List<Mii>();

    public int PlayerCount => int.Parse(Count);
    
    public int Vr
    {
        get
        {
            int evValue;
            return int.TryParse(Ev, out evValue) ? evValue : -1;
        }
    }
    
    private BitmapImage? _miiImage;

    public BitmapImage? MiiImage
    {
        get => _miiImage;
        set
        {
            if (_miiImage == value) return;
            _miiImage = value;
            OnPropertyChanged(nameof(MiiImage));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}