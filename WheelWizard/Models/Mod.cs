using System.ComponentModel;

namespace CT_MKWII_WPF.Models;

public class Mod : INotifyPropertyChanged
{
    private bool _isEnabled;
    private string _title;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value) return;
            _isEnabled = value;
            OnPropertyChanged(nameof(IsEnabled));
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ModData
{
    public bool IsEnabled { get; set; }
    public string Title { get; set; }
}
