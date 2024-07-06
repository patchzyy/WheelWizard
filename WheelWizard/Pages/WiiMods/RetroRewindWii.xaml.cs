using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Pages.WiiMods;

public partial class RetroRewindWii : UserControl
{
    public RetroRewindWii()
    {
        InitializeComponent();
        ActionButton.Content = "Install RR to SD/USB";
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        
        //make user choose between SD or USB with pop up
        var popup = new Box.USBPopupRR();
        popup.ShowDialog();
        

    }

    private void Change_Resolution(object sender, SelectionChangedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}