using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;

namespace WheelWizard.Views.Components;

public class InputField : TextBox
{
    public enum InputVariantType
    {
        Default,
        Dark
    }

    public InputField()
    {
        FontSize = 16;
        UpdateVariantStyle(Variant);
        // The issue was likely here, the compiler might not be able to infer the delegate type for Subscribe.
        // We can handle this in OnPropertyChanged instead, which is a more common pattern for property changes.
        // this.GetObservable(TextProperty).Subscribe(_ => UpdatePlaceholderVisibility());
    }

    public static readonly StyledProperty<InputVariantType> VariantProperty =
        AvaloniaProperty.Register<InputField, InputVariantType>(nameof(Variant));

    public InputVariantType Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    private void UpdateVariantStyle(InputVariantType variant)
    {
        PseudoClasses.Remove(":dark");

        switch (variant)
        {
            case InputVariantType.Dark:
                PseudoClasses.Add(":dark");
                break;
            case InputVariantType.Default:
            default:
                // Default styles are applied automatically
                break;
        }
    }

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<InputField, string>(nameof(Label));

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly StyledProperty<string> LabelTipProperty =
        AvaloniaProperty.Register<InputField, string>(nameof(LabelTip));

    public string LabelTip
    {
        get => GetValue(LabelTipProperty);
        set => SetValue(LabelTipProperty, value);
    }

    public static readonly StyledProperty<string> PlaceholderProperty =
        AvaloniaProperty.Register<InputField, string>(nameof(Placeholder));

    public string Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdateLabelVisibility();
        UpdatePlaceholderVisibility();
    }

    private void UpdateLabelVisibility()
    {
        if (this.FindControl<FormFieldLabel>("LabelElement") is { } labelElement)
        {
            labelElement.IsVisible = !string.IsNullOrEmpty(Label);
        }
    }

    private void UpdatePlaceholderVisibility()
    {
        if (this.FindControl<TextBlock>("PlaceholderBlock") is { } placeholderBlock)
        {
            placeholderBlock.IsVisible = string.IsNullOrEmpty(Text) && !IsFocused;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == VariantProperty)
        {
            UpdateVariantStyle(change.GetNewValue<InputVariantType>());
        }
        else if (change.Property == LabelProperty)
        {
            UpdateLabelVisibility();
        }
        else if (change.Property == IsFocusedProperty)
        {
            UpdatePlaceholderVisibility();
        }
        // Add this to handle Text property changes and update placeholder visibility.
        else if (change.Property == TextProperty)
        {
            UpdatePlaceholderVisibility();
        }
    }
}
