using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace WheelWizard.Views.Components;

public class LoadingIcon : TemplatedControl
{
    public LoadingIcon()
    {
        var color = Application.Current?.FindResource("Neutral900");
        if(color !=null) Foreground = new SolidColorBrush((Color)color);
    }
    
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<LoadingIcon, double>(nameof(IconSize), 20.0);

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    
    public static readonly StyledProperty<string> AdditionalTextProperty =
        AvaloniaProperty.Register<LoadingIcon, string>(nameof(AdditionalText), "");
    
    public string AdditionalText
    {
        get => GetValue(AdditionalTextProperty);
        set => SetValue(AdditionalTextProperty, value);
    }
}

