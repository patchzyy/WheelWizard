using System.ComponentModel;
using System.Windows.Controls;
using WheelWizard.Models.RRInfo;
using WheelWizard.Services.WiiManagement.SaveData;

namespace WheelWizard.WPFViews.Pages.KitchenSink;

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

