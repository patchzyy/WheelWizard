using System;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Components;

public partial class FormFieldLabel : UserControl
{
    public FormFieldLabel() => InitializeComponent();
    
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(FormFieldLabel), 
        new PropertyMetadata(string.Empty));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public static readonly DependencyProperty TipTextProperty = DependencyProperty.Register(
        nameof(TipText), typeof(string), typeof(FormFieldLabel), 
        new PropertyMetadata(string.Empty, OnTipTextChanged));


    public string TipText
    {
        get => (string)GetValue(TipTextProperty);
        set => SetValue(TipTextProperty, value);
    }

    private static void OnTipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // quick and dirty fix to set the tooltip, because for some reason
        // `Content="{Binding Text, ElementName=Root}"` seems to not work, who knows why ¯\_(ツ)_/¯
        if (d is FormFieldLabel formFieldLabel)
            formFieldLabel.FormFieldLabelTip.Content = e.NewValue;
    }
}
