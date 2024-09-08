using CT_MKWII_WPF.Services.Settings;
using System;
using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Popups;

// TODO: There should come a generic approuch to popup windows
//      - This would include one definative base class for the window for all popups
//      - This base class would include the OnClose code and stuff like that, it would also include the scale code
//      - All other things that should not be generic would be in the specific popup classes
public partial class ProgressWindow : Window
{
    private readonly bool _allowLayoutInteraction;

    public ProgressWindow(bool allowLayoutInteraction = false)
    {
        _allowLayoutInteraction = allowLayoutInteraction;
        InitializeComponent();
        
        var scaleFactor = (double)SettingsManager.WINDOW_SCALE.Get();
        Height *= scaleFactor;
        Width *= scaleFactor;
        ScaleTransform.ScaleX = scaleFactor;
        ScaleTransform.ScaleY = scaleFactor;

        Loaded += ProgressWindow_Loaded;
    }

    public void UpdateProgress(int progress, string status, string bottomText)
    {
        Dispatcher.Invoke(() =>
        {
            ProgressBar.Value = progress;
            StatusLabel.Text = status;
            BottomTextLabel.Text = bottomText;
        });
    }

    public void ChangeExtraText(string text) => ExtraTextLabel.Text = text;

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
