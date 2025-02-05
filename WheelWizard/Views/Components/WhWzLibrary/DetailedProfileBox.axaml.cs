using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System;

namespace WheelWizard.Views.Components.WhWzLibrary;

public class DetailedProfileBox : UserControl
{
    
    public static readonly StyledProperty<Bitmap?> MiiImageProperty =
        AvaloniaProperty.Register<DetailedProfileBox, Bitmap?>(nameof(MiiImage));
    public Bitmap? MiiImage
    {
        get => GetValue(MiiImageProperty);
        set => SetValue(MiiImageProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsOnlineProperty =
        AvaloniaProperty.Register<DetailedProfileBox, bool>(nameof(IsOnline));
    public bool IsOnline
    {
        get => GetValue(IsOnlineProperty);
        set => SetValue(IsOnlineProperty, value);
    }
    
    public static readonly StyledProperty<string> TotalWonProperty =
        AvaloniaProperty.Register<DetailedProfileBox, string>(nameof(TotalWon));
    public string TotalWon
    {
        get => GetValue(TotalWonProperty);
        set => SetValue(TotalWonProperty, value);
    }
    public static readonly StyledProperty<string> TotalRacesProperty =
        AvaloniaProperty.Register<DetailedProfileBox, string>(nameof(TotalRaces));
    public string TotalRaces
    {
        get => GetValue(TotalRacesProperty);
        set => SetValue(TotalRacesProperty, value);
    }
    
    public static readonly StyledProperty<string> VrProperty =
        AvaloniaProperty.Register<DetailedProfileBox, string>(nameof(Vr));
    public string Vr
    {
        get => GetValue(VrProperty);
        set => SetValue(VrProperty, value);
    }
    public static readonly StyledProperty<string> BrProperty =
        AvaloniaProperty.Register<DetailedProfileBox, string>(nameof(Br));
    public string Br
    {
        get => GetValue(BrProperty);
        set => SetValue(BrProperty, value);
    }
    
    public static readonly StyledProperty<string> FriendCodeProperty =
        AvaloniaProperty.Register<DetailedProfileBox, string>(nameof(FriendCode));
    public string FriendCode
    {
        get => GetValue(FriendCodeProperty);
        set => SetValue(FriendCodeProperty, value);
    }
    
    public static readonly StyledProperty<string> UserNameProperty =
        AvaloniaProperty.Register<DetailedProfileBox, string>(nameof(UserName));
    public string UserName
    {
        get => GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<DetailedProfileBox, bool>(nameof(IsChecked));
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
    
    public static readonly StyledProperty< EventHandler<RoutedEventArgs>?> OnCheckedProperty =
        AvaloniaProperty.Register<DetailedProfileBox,  EventHandler<RoutedEventArgs>?>(nameof(OnChecked));
    public  EventHandler<RoutedEventArgs>? OnChecked
    {
        get => GetValue(OnCheckedProperty);
        set => SetValue(OnCheckedProperty, value);
    }
    
    public static readonly StyledProperty< EventHandler<RoutedEventArgs>?> ViewRoomActionProperty =
        AvaloniaProperty.Register<DetailedProfileBox,  EventHandler<RoutedEventArgs>?>(nameof(ViewRoomAction));
    public  EventHandler<RoutedEventArgs>? ViewRoomAction
    {
        get => GetValue(ViewRoomActionProperty);
        set => SetValue(ViewRoomActionProperty, value);
    }

    private void CopyFriendCode(object? obj, EventArgs e)
    {
        TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(FriendCode);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var checkBox = e.NameScope.Find<RadioButton>("CheckBox");
        if (checkBox != null)
            checkBox.Checked += OnChecked;
        
        var viewRoomButton  = e.NameScope.Find<StandardLibrary.Button>("ViewRoomButton");
        if (viewRoomButton != null)
            viewRoomButton.Click += ViewRoomAction;
        
        var copyFcButton  = e.NameScope.Find<StandardLibrary.IconLabelButton>("CopyFcButton");
        if (copyFcButton != null) 
            copyFcButton.Click += CopyFriendCode;
    }
}

