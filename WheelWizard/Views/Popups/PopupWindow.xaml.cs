using System;
using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Popups;

public partial class PopupWindow : Window
{
    private readonly bool _allowLayoutInteraction;
    
    public PopupWindow(bool allowLayoutInteraction = false)
    {
        _allowLayoutInteraction = allowLayoutInteraction;
        InitializeComponent();
        Loaded += PopupWindow_Loaded;
    }
    
    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_allowLayoutInteraction)
            ViewUtils.GetLayout().DisableEverything();
    }
    
    protected override void OnClosed(EventArgs e)
    {
        ViewUtils.GetLayout().EnableEverything();
        base.OnClosed(e);
    }
}

