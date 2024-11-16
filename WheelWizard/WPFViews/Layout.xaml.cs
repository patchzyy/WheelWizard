using MahApps.Metro.IconPacks;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WheelWizard.Helpers;
using WheelWizard.Models.Settings;
using WheelWizard.Resources.Languages;
using WheelWizard.Services;
using WheelWizard.Services.LiveData;
using WheelWizard.Services.Settings;
using WheelWizard.Services.WiiManagement.SaveData;
using WheelWizard.Utilities.RepeatedTasks;
using WheelWizard.WPFViews.Components;
using WheelWizard.WPFViews.Pages;
using WheelWizard.WPFViews.Popups.Generic;

namespace WheelWizard.WPFViews;

public partial class Layout : Window, IRepeatedTaskListener, ISettingListener
{
    public const double WindowHeight = 876;
    public const double WindowWidth = 656;
    
    public Layout() : this(new HomePage()) { }
    public Layout(Page initialPage)
    {
        InitializeComponent();
        DataContext = this;
       
    #if RELEASE_BUILD
         KitchenSinkButton.Visibility = Visibility.Collapsed;
    #endif
    
        OnSettingChanged(SettingsManager.SAVED_WINDOW_SCALE);
        SettingsManager.WINDOW_SCALE.Subscribe(this);
       
        var completeString = Humanizer.ReplaceDynamic(Phrases.Text_MadeByString, "Patchzy", "WantToBeeMe" );
        if (completeString != null && completeString.Contains("\\n"))
        {
            var split = completeString.Split("\\n");
            MadeBy_Part1.Text = split[0];
            MadeBy_Part2.Text = split[1];
        }
        
        try
        {
            NavigateToPage(initialPage);
            InitializeManagers();
        }
        catch (Exception e)
        {
            new YesNoWindow().SetMainText(e.Message).AwaitAnswer();
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

    public void UpdatePlayerAndRoomCount(RRLiveRooms sender)
    {
        var playerCount = sender.PlayerCount;
        var roomCount = sender.RoomCount;
        PlayerCountBox.Text = playerCount.ToString();
        PlayerCountBox.TipText = playerCount switch
        {
            1 => Phrases.Hover_PlayersOnline_1,
            0 => Phrases.Hover_PlayersOnline_0,
            _ => Humanizer.ReplaceDynamic(Phrases.Hover_PlayersOnline_x, playerCount) ?? 
                 $"There are currently {playerCount} players online"
        };
        RoomCountBox.Text = roomCount.ToString();
        RoomCountBox.TipText = roomCount switch
        {
            1 => Phrases.Hover_RoomsOnline_1,
            0 => Phrases.Hover_RoomsOnline_0,
            _ => Humanizer.ReplaceDynamic(Phrases.Hover_RoomsOnline_x, roomCount) ?? 
                 $"There are currently {roomCount} rooms active"
        };
        var friends = GameDataLoader.Instance.GetCurrentFriends;
        FriendsButton.BoxText =$"{friends.Count(friend => friend.IsOnline)}/{friends.Count}";
        FriendsButton.BoxTip = friends.Count(friend => friend.IsOnline) switch
        {
            1 => Phrases.Hover_FriendsOnline_1,
            0 => Phrases.Hover_FriendsOnline_0,
            _ => Humanizer.ReplaceDynamic(Phrases.Hover_FriendsOnline_x, friends.Count(friend => friend.IsOnline)) ?? 
                 $"There are currently {friends.Count(friend => friend.IsOnline)} friends online"
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
