using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System;
using WheelWizard.Models.MiiImages;

namespace WheelWizard.Views.Components.WhWzLibrary;

public class FriendsListItem : TemplatedControl
{
    public static readonly StyledProperty<bool> IsOnlineProperty =
        AvaloniaProperty.Register<FriendsListItem, bool>(nameof(IsOnline));
    public bool IsOnline
    {
        get => GetValue(IsOnlineProperty);
        set => SetValue(IsOnlineProperty, value);
    }
    
    public static readonly StyledProperty<Mii?> MiiProperty =
        AvaloniaProperty.Register<FriendsListItem, Mii?>(nameof(Mii));
    public Mii? Mii
    {
        get => GetValue(MiiProperty);
        set => SetValue(MiiProperty, value);
    }
    
    public static readonly StyledProperty<string> TotalWonProperty =
        AvaloniaProperty.Register<FriendsListItem, string>(nameof(TotalWon));
    public string TotalWon
    {
        get => GetValue(TotalWonProperty);
        set => SetValue(TotalWonProperty, value);
    }
    public static readonly StyledProperty<string> TotalRacesProperty =
        AvaloniaProperty.Register<FriendsListItem, string>(nameof(TotalRaces));
    public string TotalRaces
    {
        get => GetValue(TotalRacesProperty);
        set => SetValue(TotalRacesProperty, value);
    }
    
    public static readonly StyledProperty<string> VrProperty =
        AvaloniaProperty.Register<FriendsListItem, string>(nameof(Vr));
    public string Vr
    {
        get => GetValue(VrProperty);
        set => SetValue(VrProperty, value);
    }
    public static readonly StyledProperty<string> BrProperty =
        AvaloniaProperty.Register<FriendsListItem, string>(nameof(Br));
    public string Br
    {
        get => GetValue(BrProperty);
        set => SetValue(BrProperty, value);
    }
    
    public static readonly StyledProperty<string> FriendCodeProperty =
        AvaloniaProperty.Register<FriendsListItem, string>(nameof(FriendCode));
    public string FriendCode
    {
        get => GetValue(FriendCodeProperty);
        set => SetValue(FriendCodeProperty, value);
    }
    
    public static readonly StyledProperty<string> UserNameProperty =
        AvaloniaProperty.Register<FriendsListItem, string>(nameof(UserName));
    public string UserName
    {
        get => GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }
    
    public static readonly StyledProperty< EventHandler<RoutedEventArgs>?> ViewRoomActionProperty =
        AvaloniaProperty.Register<FriendsListItem,  EventHandler<RoutedEventArgs>?>(nameof(ViewRoomAction));
    public  EventHandler<RoutedEventArgs>? ViewRoomAction
    {
        get => GetValue(ViewRoomActionProperty);
        set => SetValue(ViewRoomActionProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var viewRoomButton = e.NameScope.Find<StandardLibrary.Button>("ViewRoomButton");
        if (viewRoomButton != null)
            viewRoomButton.Click += ViewRoomAction;
    }
}

