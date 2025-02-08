﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using WheelWizard.Models.MiiImages;

namespace WheelWizard.Views.Components.WhWzLibrary;

public class MiiImageLoader : TemplatedControl, INotifyPropertyChanged
{
    private static bool _initializedRandomRotation = false;
    private static bool _randomRotation = false;
    
    public static readonly StyledProperty<IBrush> LoadingColorProperty =
        AvaloniaProperty.Register<MiiImageLoader, IBrush>(nameof(LoadingColor));

    public IBrush LoadingColor
    {
        get => GetValue(LoadingColorProperty);
        set => SetValue(LoadingColorProperty, value);
    }
    
    public static readonly StyledProperty<IBrush> FallBackColorProperty =
        AvaloniaProperty.Register<MiiImageLoader, IBrush>(nameof(FallBackColor));

    public IBrush FallBackColor
    {
        get => GetValue(FallBackColorProperty);
        set => SetValue(FallBackColorProperty, value);
    }
    
    public static readonly StyledProperty<MiiImageVariants.Variant> ImageVariantProperty =
        AvaloniaProperty.Register<MiiImageLoader, MiiImageVariants.Variant>(nameof(ImageVariant), 
            MiiImageVariants.Variant.DEFAULT, coerce: CoerceVariant);

    public MiiImageVariants.Variant ImageVariant
    {
        get => GetValue(ImageVariantProperty);
        set => SetValue(ImageVariantProperty, value);
    }
    
    public static readonly StyledProperty<bool> ConstraintRotationToVerticalProperty =
        AvaloniaProperty.Register<MiiImageLoader, bool>(nameof(ConstraintRotationToVertical));
    public bool ConstraintRotationToVertical
    {
        get => GetValue(ConstraintRotationToVerticalProperty);
        set => SetValue(ConstraintRotationToVerticalProperty, value);
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
    
    private static MiiImageVariants.Variant CoerceVariant(AvaloniaObject o, MiiImageVariants.Variant value)
    {
        ((MiiImageLoader)o).OnVariantChanged(value);
        return value;
    }
    private static Mii? CoerceMii(AvaloniaObject o, Mii? value)
    {
        ((MiiImageLoader)o).OnMiiChanged(value);
        return value;
    }

    private void OnVariantChanged(MiiImageVariants.Variant newValue)
    {
        ReloadImage(Mii?.GetImage(ImageVariant), Mii?.GetImage(newValue));
    }

    private void OnMiiChanged(Mii? newValue)
    {
        ReloadImage(Mii?.GetImage(ImageVariant), newValue?.GetImage(ImageVariant));
    } 

    private void ReloadImage(MiiImage? oldImage, MiiImage? newImage)
    {
        if (oldImage != null) oldImage.PropertyChanged -= NotifyMiiImageChangedInternally;
       
        if (newImage != null) 
            newImage.PropertyChanged += NotifyMiiImageChangedInternally;
        MiiImage = newImage?.Image;
        MiiLoaded = newImage?.LoadedImageSuccessfully == true;
        
        MiiChanged?.Invoke(this, EventArgs.Empty);
    }

    private void NotifyMiiImageChangedInternally(object? sender, PropertyChangedEventArgs args)
    {
        var variantedImage = Mii?.GetImage(ImageVariant);
        if (args.PropertyName != nameof(variantedImage.Image)) return;
        MiiImage = Mii?.GetImage(ImageVariant).Image;
        MiiLoaded = Mii?.GetImage(ImageVariant).LoadedImageSuccessfully == true;
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

        if (!_initializedRandomRotation)
        {
            _initializedRandomRotation = true;
           // check if today is april fools
           _randomRotation = (DateTime.Now.Month == 4) && (DateTime.Now.Day == 1);
        }
        if (_randomRotation)
        {
            // This will only happen as a joke, e.g. on april fools.
            miiImageContainer.RenderTransform = ConstraintRotationToVertical ? 
                new RotateTransform(180) : 
                new RotateTransform(new Random().NextDouble() * 360);
        }
            
            
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
