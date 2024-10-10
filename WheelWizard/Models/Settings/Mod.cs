using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CT_MKWII_WPF.Models.Settings;

public class Mod : INotifyPropertyChanged
{
    private bool _isEnabled;
    private string _title;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value) 
                return;
            
            _isEnabled = value;
            OnPropertyChanged(nameof(IsEnabled));
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) 
                return;
            
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class ModData
{
    public bool IsEnabled { get; set; }
    public string Title { get; set; }
}
