using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives; // Add this for PlacementMode
using Avalonia.Media;

namespace WheelWizard.Views.Components;

public partial class StateBox : TemplatedControl // Change to TemplatedControl
{
    public StateBox()
    {
        // No InitializeComponent() needed for TemplatedControl
    }

    // Styled properties remain the same
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<StateBox, string>(nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<StateBox, Geometry>(nameof(IconData)); // Corrected type

    public Geometry IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<StateBox, double>(nameof(IconSize), 14);

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly StyledProperty<object> TipTextProperty =
        AvaloniaProperty.Register<StateBox, object>(nameof(TipText), "Tip goes here");

    public object TipText
    {
        get => GetValue(TipTextProperty);
        set => SetValue(TipTextProperty, value);
    }

    public static readonly StyledProperty<PlacementMode> TipPlacementProperty =
        AvaloniaProperty.Register<StateBox, PlacementMode>(nameof(TipPlacement), PlacementMode.Right);

    public PlacementMode TipPlacement
    {
        get => GetValue(TipPlacementProperty);
        set => SetValue(TipPlacementProperty, value);
    }

    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<StateBox, double>(nameof(FontSize), 12);

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
}
