using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components;

public partial class MiiBall : UserControl
{
 
    public enum BallColor
    {
        Default,
        Light,
        Online,
        Red
    }
    
    public MiiBall()
    {
        InitializeComponent();
    }
 
    public static readonly DependencyProperty MiiImageProperty =
        DependencyProperty.Register(nameof(MiiImage), typeof(ImageSource), typeof(MiiBall), 
                                    new PropertyMetadata(null));
    
    public ImageSource MiiImage
    {
        get => (ImageSource)GetValue(MiiImageProperty);
        set => SetValue(MiiImageProperty, value);
    }

    
    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(double), typeof(MiiBall),
                                    new PropertyMetadata(40.0, OnBallSizeChanged));
    
    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
    
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(BallColor), typeof(MiiBall),
                                    new PropertyMetadata(BallColor.Default));

    public BallColor Color
    {
        get => (BallColor)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    
    private static void OnBallSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MiiBall control) return;
        
        var newSize = (double)e.NewValue;
        control.Border.Width = newSize;
        control.Border.Height = newSize;
        
        control.EllipseGeometry.Center = new Point(newSize / 2, newSize / 2);
        control.EllipseGeometry.RadiusX = newSize / 2;
        control.EllipseGeometry.RadiusY = newSize / 2;
    }
}
