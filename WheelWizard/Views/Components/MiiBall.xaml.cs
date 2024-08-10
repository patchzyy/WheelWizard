using CT_MKWII_WPF.Models.RRInfo;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Views.Components;

// TODO: I think it is also possible to directly bind the Mii values
//       instead of setting the image to an image and binding that
//       That would be worth creating, would require a bit less code
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
        Unloaded += MiiBall_Unloaded;
    }
 
    public static readonly DependencyProperty MiiImageProperty =
        DependencyProperty.Register(nameof(MiiImage), typeof(ImageSource), typeof(MiiBall), 
                                    new PropertyMetadata(new BitmapImage()));
    
    public ImageSource? MiiImage
    {
        get => (ImageSource)GetValue(MiiImageProperty);
        private set => SetValue(MiiImageProperty, value);
    }

    
    public static readonly DependencyProperty ImageLoadingSuccessProperty =
        DependencyProperty.Register(nameof(ImageLoadingSuccess), typeof(bool), typeof(MiiBall),
                                    new PropertyMetadata(false));

    public bool ImageLoadingSuccess
    {
        get => (bool)GetValue(ImageLoadingSuccessProperty);
        private set => SetValue(ImageLoadingSuccessProperty, value);
    }
    
    public static readonly DependencyProperty MiiProperty =
        DependencyProperty.Register(nameof(Mii), typeof(Mii), typeof(MiiBall), 
                                    new PropertyMetadata(null, OnMiiChanged));
    
    public Mii? Mii
    {
        get => (Mii)GetValue(MiiProperty);
        set => SetValue(MiiProperty, value);
    }

    
    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(double), typeof(MiiBall),
                                    new PropertyMetadata(0.0, OnBallSizeChanged));
    
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
    
    private static void OnMiiChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MiiBall control) return;
        
        // We unsubscribe This ball from the previous Mii PropertyChanged event
        // and subscribe it to the new Mii PropertyChanged event
        if (e.OldValue is Mii oldMii)
            oldMii.PropertyChanged -= control.OnMiiPropertyChanged;

        if (e.NewValue is Mii newMii)
        {
            newMii.PropertyChanged += control.OnMiiPropertyChanged;
            control.UpdateMiiImage(newMii);
        }
        else
        {
            control.MiiImage = new BitmapImage();
            control.ImageLoadingSuccess = false;
        }
    }
    
    private void OnMiiPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Mii.Image))
            UpdateMiiImage(Mii);
    }
    
    private void UpdateMiiImage(Mii? mii)
    {
        if (mii == null)
        {
            MiiImage = null;
            ImageLoadingSuccess = false;
        }
        else
        {
            MiiImage = mii.Image;
            ImageLoadingSuccess = mii.LoadedImageSuccessfully;
        }
    }
    
    private void MiiBall_Unloaded(object sender, RoutedEventArgs e)
    {
        if (Mii != null) 
            Mii.PropertyChanged -= OnMiiPropertyChanged;
        Unloaded -= MiiBall_Unloaded;
    }
}
