using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CT_MKWII_WPF.Utils;
using CT_MKWII_WPF.Views.Pages;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CT_MKWII_WPF.Views;

public partial class Layout : Window
{
    // ADD HERE THE PAGED YOU WANT TO NAVIGATE TO
    private void DashboardPage_Navigate(object _, RoutedEventArgs e) => NavigateToPage(new Dashboard());
    private void SettingsPage_Navigate(object _, RoutedEventArgs e) => NavigateToPage(new SettingsPage());
    private void ModsPage_Navigate(object _, RoutedEventArgs e) => NavigateToPage(new ModsPage());
    private void KitchenSink_Navigate(object _, RoutedEventArgs e) => NavigateToPage(new KitchenSink());
    private void RoomsButton_Navigate(object sender, RoutedEventArgs e) => NavigateToPage(new RoomsPage());
    
    public void NavigateToPage(Page page)
    {
        ContentArea.Navigate(page);
        ContentArea.NavigationService.RemoveBackEntry();
    }
    
    public Layout()
    {
        InitializeComponent();
      
        Dashboard myPage = new Dashboard();
        NavigateToPage(myPage);
        populatePlayerText();
    }

    public async void populatePlayerText()
    {
        var rrinfo = await RRLiveInfo.getCurrentGameData();
        PlayerCountText.Text = "Players online: " + RRLiveInfo.GetCurrentOnlinePlayers(rrinfo).ToString();
        var periodicTimer= new PeriodicTimer(TimeSpan.FromSeconds(20));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            rrinfo = await RRLiveInfo.getCurrentGameData();
            PlayerCountText.Text = "Players online: " + RRLiveInfo.GetCurrentOnlinePlayers(rrinfo).ToString();
        }
        
    }
    
    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }
    
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void Discord_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://discord.gg/vZ7T2wJnsq",
            UseShellExecute = true
        });
    }

    private void Github_Click(object sender, RoutedEventArgs e)
    {  
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://github.com/patchzyy/WheelWizard",
            UseShellExecute = true
        });
    }


}
