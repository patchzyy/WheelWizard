using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace WheelWizard.Views.Components;

public class FormFieldLabel : UserControl
{   
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<FormFieldLabel, string>(nameof(Text)); // Add a default value here
   
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public static readonly StyledProperty<string> TipTextProperty =
        AvaloniaProperty.Register<FormFieldLabel, string>(nameof(TipText)); // Add a default value here
   
    public string TipText
    {
        get => GetValue(TipTextProperty);
        set => SetValue(TipTextProperty, value);
    }
}

