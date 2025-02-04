using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System;

namespace WheelWizard.Views.Components.WhWzLibrary;

public class DetailedProfileBox : TemplatedControl
{
    
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<DetailedProfileBox, bool>(nameof(IsChecked));

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
    
    public static readonly StyledProperty< EventHandler<RoutedEventArgs>?> OnCheckedProperty =
        AvaloniaProperty.Register<DetailedProfileBox,  EventHandler<RoutedEventArgs>?>(nameof(OnChecked));

    public  EventHandler<RoutedEventArgs>? OnChecked
    {
        get => GetValue(OnCheckedProperty);
        set => SetValue(OnCheckedProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var checkBox = e.NameScope.Find<RadioButton>("CheckBox");
        if (checkBox != null)
            checkBox.Checked += OnChecked;
    }
}

