using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CT_MKWII_WPF.Classes;
using CT_MKWII_WPF.Utils;
using CT_MKWII_WPF.Views.Components;
using CT_MKWII_WPF.Views.Pages;

namespace CT_MKWII_WPF.Views;

public partial class Layout : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    public Layout()
    {
        InitializeComponent();
        DataContext = this;
        NavigateToPage(new Dashboard());
        PopulatePlayerText();
        // Create and start the LiveAlertsManager
        LiveAlertsManager.Start(StatusIcon, UpdateStatusMessage);
        
        var statusIconBorder = StatusIcon.Parent as Border;
        if (statusIconBorder != null)
            statusIconBorder.ToolTip = null;
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
    
    private string _statusMessage;
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage == value) return;
            _statusMessage = value;
            OnPropertyChanged();
        }
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private void UpdateStatusMessage(string message) => Dispatcher.Invoke(() => StatusMessage = message);

    public async void PopulatePlayerText()
    {
        var rrInfo = await RRLiveInfo.getCurrentGameData();
        UpdatePlayerAndRoomCount(RRLiveInfo.GetCurrentOnlinePlayers(rrInfo), rrInfo.Rooms.Count);
        
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(20));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            rrInfo = await RRLiveInfo.getCurrentGameData();
            UpdatePlayerAndRoomCount(RRLiveInfo.GetCurrentOnlinePlayers(rrInfo), rrInfo.Rooms.Count);
        }
    }
    
    public void UpdatePlayerAndRoomCount(int playerCount, int roomCount)
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
    
    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
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
