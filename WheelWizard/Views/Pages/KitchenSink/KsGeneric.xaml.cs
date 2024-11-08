using CT_MKWII_WPF.Views.Popups;
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
        new YesNoWindow().SetMainText("ButtonClicked");

    private void InputButton_OnClick(object _, RoutedEventArgs e) =>
        new YesNoWindow().SetMainText("InputFieldContains " +MyInputField.Text);
}

