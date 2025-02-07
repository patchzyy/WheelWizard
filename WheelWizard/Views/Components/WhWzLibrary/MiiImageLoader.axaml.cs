using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using WheelWizard.Models.RRInfo;

namespace WheelWizard.Views.Components.WhWzLibrary;

public class MiiImageLoader : TemplatedControl, INotifyPropertyChanged
{
    public static readonly StyledProperty<IBrush> FallBackColorProperty =
        AvaloniaProperty.Register<MiiImageLoader, IBrush>(nameof(FallBackColor));

    public IBrush FallBackColor
    {
        get => GetValue(FallBackColorProperty);
        set => SetValue(FallBackColorProperty, value);
    }
    
    private static readonly StyledProperty<bool> HeightAsScalingReferenceProperty =
        AvaloniaProperty.Register<MiiImageLoader, bool>(nameof(HeightAsScalingReference), true);
    public bool HeightAsScalingReference
    {
        get => GetValue(HeightAsScalingReferenceProperty);
        set => SetValue(HeightAsScalingReferenceProperty, value);
    }
    
    public static readonly StyledProperty<bool> MiiLoadedProperty =
        AvaloniaProperty.Register<MiiImageLoader, bool>(nameof(MiiLoaded));
    private bool MiiLoaded
    {
        get => GetValue(MiiLoadedProperty);
        set
        {
            SetValue(MiiLoadedProperty, value);
            OnPropertyChanged(nameof(MiiLoaded));
        }
    }
    
    public static readonly StyledProperty<Bitmap?> MiiImageProperty =
        AvaloniaProperty.Register<MiiImageLoader, Bitmap?>(nameof(MiiImage));
    private Bitmap? MiiImage
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
        MiiLoaded = newValue?.LoadedImageSuccessfully == true;
        
        MiiChanged?.Invoke(this, EventArgs.Empty);
    }

    private void NotifyMiiImageChangedInternally(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(Mii.Image)) return;
        MiiImage = Mii?.Image;
        MiiLoaded = Mii?.LoadedImageSuccessfully == true;
    }
    
    public event EventHandler MiiChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var miiImageContainer = e.NameScope.Find<Grid>("MiiImageContainer");
        if (miiImageContainer == null) return;

        var loadingIcon = e.NameScope.Find<StandardLibrary.LoadingIcon>("MiiLoadingIcon");
        var fallbackIcon = e.NameScope.Find<PathIcon>("MiiFallBackIcon");
        
        Dispatcher.UIThread.Post(() =>
        {
            var newValue = HeightAsScalingReference ? 
                miiImageContainer.Bounds.Height :
                miiImageContainer.Bounds.Width;
            if (loadingIcon != null)
                loadingIcon.IconSize = newValue*0.7;

            if (fallbackIcon == null) return;
            fallbackIcon.Width = newValue * 0.85;
            fallbackIcon.Height = newValue * 0.85;
            fallbackIcon.Margin = new Thickness(0,0,0,-newValue * 0.05);

        }, DispatcherPriority.Render); 
    }
}
