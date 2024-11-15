using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WheelWizard.Views.Components;

public partial class Dropdown : ComboBox
{
    public Dropdown()
    {
        InitializeComponent();
        FontSize = 16.0;
        MaxDropDownHeight = 225.0;
        Placeholder = "Select an option";
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateLabelVisibility();
    }
    
    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(Dropdown),
                                    new PropertyMetadata(string.Empty));

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }
    
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(Dropdown),
                                    new PropertyMetadata(string.Empty, OnLabelChanged));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Dropdown dropdown)
            dropdown.UpdateLabelVisibility();
    }

    private void UpdateLabelVisibility()
    {
        if (Template.FindName("LabelElement", this) is FormFieldLabel labelElement)
            labelElement.Visibility = string.IsNullOrEmpty(Label) ? Visibility.Collapsed : Visibility.Visible;
    }

    public static readonly DependencyProperty LabelTipProperty =
        DependencyProperty.Register(nameof(LabelTip), typeof(string), typeof(Dropdown),
                                    new PropertyMetadata(string.Empty));

    public string LabelTip
    {
        get => (string)GetValue(LabelTipProperty);
        set => SetValue(LabelTipProperty, value);
    }
    
    private void DropDown_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not Border border)
            return;
        
        border.Clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, border.ActualWidth, border.ActualHeight),
            RadiusX = 6,
            RadiusY = 6
        };
    }
}
