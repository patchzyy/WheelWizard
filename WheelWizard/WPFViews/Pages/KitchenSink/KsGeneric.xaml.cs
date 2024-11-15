using System.Windows;
using System.Windows.Controls;
using WheelWizard.Views.Popups;

namespace WheelWizard.Views.Pages.KitchenSink;

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

