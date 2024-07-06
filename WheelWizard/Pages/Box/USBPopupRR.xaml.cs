using System.Windows;

namespace CT_MKWII_WPF.Pages.Box;

public partial class USBPopupRR : Window
{
    public USBPopupRR()
    {
        InitializeComponent();
    }

    private void SD_Click(object sender, RoutedEventArgs e)
    {
        RetroRewindInstaller.InstallRRToSD();
    }

    private void USB_Click(object sender, RoutedEventArgs e)
    {
        RetroRewindInstaller.InstallRRToUSB();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}