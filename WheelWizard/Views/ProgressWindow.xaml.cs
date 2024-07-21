using System;
using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views;

public partial class ProgressWindow : Window
{
    private readonly bool _allowLayoutInteraction;
    
    public ProgressWindow(bool allowLayoutInteraction = false)
    {
        _allowLayoutInteraction = allowLayoutInteraction;
        InitializeComponent();
        
        Loaded += ProgressWindow_Loaded;
    }

    public void UpdateProgress(int progress, string status, string bottomText)
    {
        ProgressBar.Value = progress;
        StatusLabel.Text = status;
        BottomTextLabel.Text = bottomText;
    }
    
    public void ChangeExtraText(string text) =>  ExtraTextLabel.Text = text;
    
    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    
    private void ProgressWindow_Loaded(object sender, RoutedEventArgs e)
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