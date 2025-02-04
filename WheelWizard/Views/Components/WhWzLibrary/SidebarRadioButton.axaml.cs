using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using WheelWizard.Views.Pages;

namespace WheelWizard.Views.Components.WhWzLibrary;

public partial class SidebarRadioButton : RadioButton
{
    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<SidebarRadioButton, Geometry>(nameof(IconData));

    public Geometry IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public static readonly StyledProperty<Type?> PageTypeProperty =
        AvaloniaProperty.Register<SidebarRadioButton, Type?>(nameof(PageType));

    public Type? PageType
    {
        get => GetValue(PageTypeProperty);
        set => SetValue(PageTypeProperty, value);
    }

    public static readonly StyledProperty<string> BoxTextProperty =
        AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(BoxText));

    public string BoxText
    {
        get => GetValue(BoxTextProperty);
        set => SetValue(BoxTextProperty, value);
    }

    public static readonly StyledProperty<string> BoxTipProperty =
        AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(BoxTip));

    public string BoxTip
    {
        get => GetValue(BoxTipProperty);
        set => SetValue(BoxTipProperty, value);
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        
        PageType ??= typeof(NotFoundPage);
        if (Activator.CreateInstance(PageType) is not UserControl page)
            page = (UserControl)Activator.CreateInstance(typeof(NotFoundPage))!;
        ViewUtils.NavigateToPage(page);
    }
}
