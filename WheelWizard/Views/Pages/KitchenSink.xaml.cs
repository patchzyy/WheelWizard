using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CT_MKWII_WPF.Views.Pages;


namespace CT_MKWII_WPF.Views.Pages;

public partial class KitchenSink : Page
{
    public KitchenSink()
    {
        InitializeComponent();
    }
    private void Button_OnClick(object _, RoutedEventArgs e) => 
        MessageBox.Show("Button Clicked");

    private void InputButton_OnClick(object _, RoutedEventArgs e) =>
        MessageBox.Show($"Input field contains: {MyInputField.Text}");
}
