using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.WiiManagement.SaveData;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages.KitchenSink;

public partial class KsOnline : UserControl, INotifyPropertyChanged
{
    private Mii? _mii;
    public Mii? Mii
    {
        get => _mii;
        set
        {
            _mii = value;
            OnPropertyChanged(nameof(Mii));
        }
    }
    
    public KsOnline()
    {
        InitializeComponent();
        DataContext = this;
        Mii = GameDataLoader.Instance.GetCurrentUser.Mii;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

