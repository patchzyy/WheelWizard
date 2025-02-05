using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using System;
using System.ComponentModel;
using WheelWizard.Models.RRInfo;

namespace WheelWizard.Views.Components.WhWzLibrary;

public class MiiImageLoader : TemplatedControl, INotifyPropertyChanged
{
    public static readonly StyledProperty<Bitmap?> MiiImageProperty =
        AvaloniaProperty.Register<MiiImageLoader, Bitmap?>(nameof(MiiImage));
    public Bitmap? MiiImage
    {
        get => GetValue(MiiImageProperty);
        set
        {
            SetValue(MiiImageProperty, value);
            OnPropertyChanged(nameof(MiiImage));
        }
    }
    
    public static readonly StyledProperty<Mii?> MiiProperty =
        AvaloniaProperty.Register<MiiImageLoader, Mii?>(nameof(Mii), coerce: CoerceMii);
    public Mii? Mii
    {
        get => GetValue(MiiProperty);
        set => SetValue(MiiProperty, value);
    }
    
    private static Mii? CoerceMii(AvaloniaObject o, Mii? value)
    {
        ((MiiImageLoader)o).OnMiiChanged(value);
        return value;
    }

    private void OnMiiChanged(Mii? newValue)
    {
        if (Mii != null) Mii.PropertyChanged -= NotifyMiiImageChangedInternally;
       
        if (newValue != null) 
            newValue.PropertyChanged += NotifyMiiImageChangedInternally;
        MiiImage = newValue?.Image;
        
        MiiChanged?.Invoke(this, EventArgs.Empty);
    }

    private void NotifyMiiImageChangedInternally(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(Mii.Image))
            MiiImage = Mii?.Image;
    }
    
    public event EventHandler MiiChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
