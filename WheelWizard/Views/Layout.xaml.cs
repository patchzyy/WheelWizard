using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.LiveData;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement.SaveData;
using CT_MKWII_WPF.Utilities.RepeatedTasks;
using CT_MKWII_WPF.Views.Components;
using CT_MKWII_WPF.Views.Pages;
using CT_MKWII_WPF.Views.Popups;
using MahApps.Metro.IconPacks;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views;

public partial class Layout : Window, IRepeatedTaskListener, ISettingListener
{
    public readonly double WindowHeight = 876;
    public readonly double WindowWidth = 656;
    
    public Layout()
    {
        InitializeComponent();
        DataContext = this;

        OnSettingChanged(SettingsManager.SAVED_WINDOW_SCALE);
        
        try
        {
            NavigateToPage(new Dashboard());
            InitializeManagers();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }
    }
    
    public void OnSettingChanged(Setting setting)
    {
        // Note that this method will also be called whenever the setting changes
        var scaleFactor = (double)setting.Get();
        Height = WindowHeight * scaleFactor;
        Width = WindowWidth * scaleFactor;
        ScaleTransform.ScaleX = scaleFactor;
        ScaleTransform.ScaleY = scaleFactor;
    }
    
    private void InitializeManagers()
    {
        LiveAlertsManager.Instance.Start(); // Temporary code, should be moved to a more appropriate location
        LiveAlertsManager.Instance.Subscribe(this);
        RRLiveRooms.Instance.Start(); // Temporary code, should be moved to a more appropriate location
        RRLiveRooms.Instance.Subscribe(this);
        GameDataLoader.Instance.Start(); // Temporary code, should be moved to a more appropriate location
        GameDataLoader.Instance.Subscribe(this);
    }
    
    public void NavigateToPage(Page page)
    {
        ContentArea.Navigate(page);
        ContentArea.NavigationService.RemoveBackEntry();

        foreach (var child in SidePanelButtons.Children)
        {
            if (child is SidebarRadioButton button)
                button.IsChecked = button.PageType == page.GetType();
        }

        var window = new YesNoWindow();
        window.SetMainText("Do you want to save your progress?");
        window.ShowDialog();
    }

    public void OnUpdate(RepeatedTaskManager sender)
    {
        switch (sender)
        {
            case RRLiveRooms liveRooms:
                UpdatePlayerAndRoomCount(liveRooms);
                break;
            case LiveAlertsManager liveAlerts:
                UpdateLiveAlert(liveAlerts);
                break;
            
        }
    }

    private void UpdatePlayerAndRoomCount(RRLiveRooms sender)
    {
        var playerCount = sender.PlayerCount;
        var roomCount = sender.RoomCount;
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
        var friends = GameDataLoader.Instance.GetCurrentFriends;
        FriendsButton.BoxText =$"{friends.Count(friend => friend.IsOnline)}/{friends.Count}";
        FriendsButton.BoxTip = friends.Count(friend => friend.IsOnline) switch
        {
            1 => "There is currently 1 friend online",
            0 => "There are currently no friends online",
            _ => $"There are currently {friends.Count(friend => friend.IsOnline)} friends online"
        };
    }

    private void UpdateLiveAlert(LiveAlertsManager sender)
    {

        if ((string)LiveAlertToolTip.Content == sender.StatusMessage) return;
        LiveAlertToolTip.Content = sender.StatusMessage;
        LiveAlert.IconPack = "FontAwesome";
        SolidColorBrush? brush = null;
        switch (sender.StatusMessageType.ToLower())
        {
            case "party":
                LiveAlert.IconPack = "Material";
                LiveAlert.IconKind = PackIconMaterialKind.PartyPopper;
                brush = Application.Current.FindResource("PartyColor") as SolidColorBrush;
                break;
            case "info":
                LiveAlert.IconKind = PackIconFontAwesomeKind.CircleInfoSolid;
                brush = Application.Current.FindResource("InfoColor") as SolidColorBrush;
                break;
            case "warning":
            case "alert":
                LiveAlert.IconKind = PackIconFontAwesomeKind.CircleExclamationSolid;
                brush = Application.Current.FindResource("WarningColor") as SolidColorBrush;
                break;
            case "error":
                LiveAlert.IconKind = PackIconFontAwesomeKind.CircleXmarkSolid;
                brush = Application.Current.FindResource("ErrorColor") as SolidColorBrush;
                break;
            case "success":
                LiveAlert.IconKind = PackIconFontAwesomeKind.CircleCheckSolid;
                brush = Application.Current.FindResource("SuccessColor") as SolidColorBrush;
                break;
            default:
                LiveAlert.IconPack = "";
                LiveAlert.IconKind = "";
                break;
        }

        // We never actually want Brushes.Gray,
        // but just in case the resource is missing for some unknown reason, we still want to display the icon
        LiveAlert.ForegroundColor = brush ?? Brushes.Gray;
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
            FileName = Endpoints.WhWzDiscordUrl,
            UseShellExecute = true
        });
    }

    private void Github_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = Endpoints.WhWzGithubUrl,
            UseShellExecute = true
        });
    }
    
    private void Support_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = Endpoints.SupportLink,
            UseShellExecute = true
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        RepeatedTaskManager.CancelAllTasks();
    }

    public void DisableEverything()
    {
        DisabledDarkenEffect.Visibility = Visibility.Visible;
        CompleteGrid.IsEnabled = false;
    }

    public void EnableEverything()
    {
        CompleteGrid.IsEnabled = true;
        DisabledDarkenEffect.Visibility = Visibility.Collapsed;
    } 
}
