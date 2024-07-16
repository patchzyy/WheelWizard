using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Classes;

public class RoomInfo
{
    public string Id { get; set; }
    public string Game { get; set; }
    public DateTime Created { get; set; }
      
    public string Type { get; set; }
    public bool Suspend { get; set; }
    public string Host { get; set; }
    public string Rk { get; set; }
    public Dictionary<string, Player> Players { get; set; }

    public class Player : INotifyPropertyChanged
    {
        public string Count { get; set; }
        public string Pid { get; set; }
        public string Name { get; set; }
        public string ConnMap { get; set; }
        public string ConnFail { get; set; }
        public string Suspend { get; set; }
        public string Fc { get; set; }
        public string Ev { get; set; } = "--";
        public string Eb { get; set; }
        public List<Mii> Mii { get; set; }

        private BitmapImage _miiImage;
        public BitmapImage MiiImage
        {
            get => _miiImage;
            set
            {
                if (_miiImage != value)
                {
                    _miiImage = value;
                    OnPropertyChanged(nameof(MiiImage));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Mii
    {
        public string Data { get; set; }
        public string Name { get; set; }
    }
}