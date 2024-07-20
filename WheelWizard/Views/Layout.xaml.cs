using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.Networking;
using CT_MKWII_WPF.Views.Components;
using CT_MKWII_WPF.Views.Pages;
using MahApps.Metro.IconPacks;

namespace CT_MKWII_WPF.Views;

public partial class Layout : Window
{
    public Layout()
    {
        InitializeComponent();
        DataContext = this;
        NavigateToPage(new Dashboard());
        PopulatePlayerText();
        LiveAlertsManager.Start(UpdateStatusIcon);

        // loadplayerdata();
    }

    // public static void loadplayerdata()
    // {
    //     var data = new GameDataLoader();
    //     data.LoadGameData();
    //     var gameData = data.GameData;
    // }

    public void NavigateToPage(Page page)
    {
        ContentArea.Navigate(page);
        ContentArea.NavigationService.RemoveBackEntry();

        foreach (var child in SidePanelButtons.Children)
        {
            if (child is SidebarRadioButton button)
                button.IsChecked = button.PageType == page.GetType();
        }
    }

    private async void PopulatePlayerText()
    {
        var rrInfo = await RRLiveInfo.GetCurrentGameData();
        UpdatePlayerAndRoomCount(RRLiveInfo.GetCurrentOnlinePlayers(rrInfo), rrInfo.Rooms.Count);

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(20));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            rrInfo = await RRLiveInfo.GetCurrentGameData();
            UpdatePlayerAndRoomCount(RRLiveInfo.GetCurrentOnlinePlayers(rrInfo), rrInfo.Rooms.Count);
        }
    }

    private void UpdatePlayerAndRoomCount(int playerCount, int roomCount)
    {
        PlayerCountBox.Text = playerCount.ToString();
        PlayerCountBox.TipText = playerCount switch
        {
            1 => "There is currently 1 player online",
            0 => "There are currently no players online",
            _ => $"There are currently {playerCount} players online"
        };
        RoomCountBox.Text = roomCount.ToString();
        RoomCountBox.TipText = roomCount switch
        {
            1 => "There is currently 1 room active",
            0 => "There are currently no rooms active",
            _ => $"There are currently {roomCount} rooms active"
        };
    }

    private void UpdateStatusIcon(string messageType, string message)
    {
        if ((string)StatusIconToolTip.Content == message) return;
        StatusIconToolTip.Content = message;
        StatusIcon.IconPack = "FontAwesome";
        SolidColorBrush? brush = null;
        switch (messageType.ToLower())
        {
            case "party":
                StatusIcon.IconPack = "Material";
                StatusIcon.IconKind = PackIconMaterialKind.PartyPopper;
                brush = Application.Current.FindResource("PartyColor") as SolidColorBrush;
                break;
            case "info":
                StatusIcon.IconKind = PackIconFontAwesomeKind.CircleInfoSolid;
                brush = Application.Current.FindResource("InfoColor") as SolidColorBrush;
                break;
            case "warning":
            case "alert":
                StatusIcon.IconKind = PackIconFontAwesomeKind.CircleExclamationSolid;
                brush = Application.Current.FindResource("WarningColor") as SolidColorBrush;
                break;
            case "error":
                StatusIcon.IconKind = PackIconFontAwesomeKind.CircleXmarkSolid;
                brush = Application.Current.FindResource("ErrorColor") as SolidColorBrush;
                break;
            case "success":
                StatusIcon.IconKind = PackIconFontAwesomeKind.CircleCheckSolid;
                brush = Application.Current.FindResource("SuccessColor") as SolidColorBrush;
                break;
            default:
                StatusIcon.IconPack = "";
                StatusIcon.IconKind = "";
                break;
        }

        // We never actually want Brushes.Gray,
        // but just in case the resource is missing for some unknown reason, we still want to display the icon
        StatusIcon.ForegroundColor = brush ?? Brushes.Gray;
    }
    
    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) 
            DragMove();
    }
    
    private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

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
 
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        LiveAlertsManager.Stop();
    }
}