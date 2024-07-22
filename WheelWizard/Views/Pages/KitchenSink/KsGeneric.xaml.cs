using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages.KitchenSink;

public partial class KsGeneric : UserControl
{
    public KsGeneric()
    {
        InitializeComponent();
    }
    
    private void Button_OnClick(object _, RoutedEventArgs e) =>
        MessageBox.Show("Button Clicked");

    private void InputButton_OnClick(object _, RoutedEventArgs e) =>
        MessageBox.Show($"Input field contains: {MyInputField.Text}");
}

