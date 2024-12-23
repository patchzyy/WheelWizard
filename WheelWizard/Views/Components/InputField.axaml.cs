using Avalonia;
using Avalonia.Controls;

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
    }
    
    public static readonly StyledProperty<InputVariantType> VariantProperty =
        AvaloniaProperty.Register<InputField,InputVariantType>(nameof(Variant));
   
    public InputVariantType Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }
    
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<InputField,string>(nameof(Label));
   
    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    
    public static readonly StyledProperty<string> LabelTipProperty =
        AvaloniaProperty.Register<InputField,string>(nameof(LabelTip));
   
    public string LabelTip
    {
        get => GetValue(LabelTipProperty);
        set => SetValue(LabelTipProperty, value);
    }
    
    
    public static readonly StyledProperty<string> PlaceholderProperty =
        AvaloniaProperty.Register<InputField,string>(nameof(Placeholder));
   
    public string Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }
}
