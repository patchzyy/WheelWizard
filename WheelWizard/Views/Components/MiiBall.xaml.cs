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
    public enum BallVariantType
    {
        Default,
        Light
    }
    
    public enum BallPlayerState
    {
        Default,
        Online,
        Red
    }
    
    public enum PlayerWinPosition
    {
        None,
        First,
        Second,
        Third
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
    

    public static readonly DependencyProperty VariantProperty =
        DependencyProperty.Register(nameof(Variant), typeof(BallVariantType), typeof(MiiBall),
                                    new PropertyMetadata(BallVariantType.Default));

    public BallVariantType Variant
    {
        get => (BallVariantType)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }
    
    public static readonly DependencyProperty PlayerStateProperty =
        DependencyProperty.Register(nameof(PlayerState), typeof(BallPlayerState), typeof(MiiBall),
                                    new PropertyMetadata(BallPlayerState.Default));

    public BallPlayerState PlayerState
    {
        get => (BallPlayerState)GetValue(PlayerStateProperty);
        set => SetValue(PlayerStateProperty, value);
    }
    
    public static readonly DependencyProperty PlayerWinPositionProperty =
        DependencyProperty.Register(nameof(WinPosition), typeof(PlayerWinPosition), typeof(MiiBall),
                                    new PropertyMetadata(PlayerWinPosition.None));

    public PlayerWinPosition WinPosition
    {
        get => (PlayerWinPosition)GetValue(PlayerWinPositionProperty);
        set => SetValue(PlayerWinPositionProperty, value);
    }
    
    private static void OnBallSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // I AM SORRY IN ADVANCE. This method is full of magic numbers. And they are really magic (っ °Д °;)っ
        if (d is not MiiBall control) return;
        // Note that the figma page design is based around a 40x40 size. So we need to calculate the border width
        // based around a size of 40.0
        var borderWidth = 3.0;
        var realBorderSize = (double)e.NewValue * (borderWidth / 40.0);
        var newInnerSize = (double)e.NewValue - realBorderSize*2;
        control.InnerCircle.Width = newInnerSize;
        control.InnerCircle.Height = newInnerSize;
        
        control.EllipseGeometry.Center = new Point(newInnerSize / 2, newInnerSize / 2);
        control.EllipseGeometry.RadiusX = newInnerSize / 2;
        control.EllipseGeometry.RadiusY = newInnerSize / 2;
        
        control.StateIcon.BorderThickness = new Thickness(realBorderSize);
        control.StateIcon.Width = newInnerSize/2.5; //State icon is the ball that indicates if the player is online/offline
        control.StateIcon.Height = newInnerSize/2.5; // devided by 2.5 since that felt a good size
        
        control.BadgeIconOuter.Width = newInnerSize/1.8; // BadeIconOuter is the font awsome icon called "award"
        control.BadgeIconOuter.Height = newInnerSize/1.8; // 1.8 felt like a good size
        
        control.BadgeIconInner.Width = newInnerSize/3.2; // BadeIconInner is trying to fill the inside of that font awsome icon
        control.BadgeIconInner.Height = newInnerSize/3.2; // 3.2 felt like a good size
        control.BadgeIconInner.Margin = new Thickness(-newInnerSize/50.0, newInnerSize/6.0, 0,0);
        // Note that his margin is also based on literally nothing. I just put a random value, looked at the result, and adjusted it again. This like 8 times
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
