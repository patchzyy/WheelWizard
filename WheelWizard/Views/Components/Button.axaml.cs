using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;

namespace WheelWizard.Views.Components;

public partial class Button : UserControl
{
    private static readonly StyledProperty<IBrush> ForegroundProperty 
        = AvaloniaProperty.Register<Button, IBrush>(nameof(Foreground), Brushes.Black);
    
    public static readonly StyledProperty<ButtonsVariantType> VariantProperty =
        AvaloniaProperty.Register<Button, ButtonsVariantType>(nameof(Variant), ButtonsVariantType.Default);
    
    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<Button, Geometry>(nameof(IconData));
    
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<Button, double>(nameof(IconSize), 20.0);
    
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<Button, string>(nameof(Text), string.Empty);
    
    public static readonly StyledProperty<System.Windows.Input.ICommand> CommandProperty =
        AvaloniaProperty.Register<Button, System.Windows.Input.ICommand>(nameof(Command));
    
    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<Button, object>(nameof(CommandParameter));
    
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<Button, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);
    
    public enum ButtonsVariantType
    {
        Primary,
        Secondary,
        Default,
        Danger
    }
    
    public Button()
    {
        InitializeComponent();
        UpdateStyleClasses(Variant);
    }

    public ButtonsVariantType Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public Geometry IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public IBrush Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }
    
    public System.Windows.Input.ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public event EventHandler<RoutedEventArgs> Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    private void UpdateStyleClasses(ButtonsVariantType variant)
    {
        Classes.Remove("primary");
        Classes.Remove("secondary");
        Classes.Remove("danger");
        Classes.Remove("default");

        switch (variant)
        {
            case ButtonsVariantType.Primary:
                Classes.Add("primary");
                break;
            case ButtonsVariantType.Secondary:
                Classes.Add("secondary");
                break;
            case ButtonsVariantType.Danger:
                Classes.Add("danger");
                break;
            default:
                Classes.Add("default");
                break;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == VariantProperty)
        {
            UpdateStyleClasses(change.GetNewValue<ButtonsVariantType>());
        }
    }

    protected virtual void OnClick()
    {
        var newEventArgs = new RoutedEventArgs(ClickEvent);

        RaiseEvent(newEventArgs);

        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }

    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            OnClick();
        }
    }
}
