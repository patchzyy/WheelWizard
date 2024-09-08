using CT_MKWII_WPF.Services.Settings;
using System;
using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Popups;

// TODO: There should come a generic approuch to popup windows
//      - This would include one definative base class for the window for all popups
//      - This base class would include the OnClose code and stuff like that, it would also include the scale code
//      - All other things that should not be generic would be in the specific popup classes
public partial class YesNoMessagebox : Window
{
    public bool Result { get; private set; }
    
    public YesNoMessagebox(string mainText, string yesText, string noText, string extraText = "", string bottomText = "")
    {
        InitializeComponent();
        
        var scaleFactor = (double)SettingsManager.WINDOW_SCALE.Get();
        Height *= scaleFactor;
        Width *= scaleFactor;
        ScaleTransform.ScaleX = scaleFactor;
        ScaleTransform.ScaleY = scaleFactor;
                
        YesButton.Text = yesText;
        NoButton.Text = noText;
        MainTextBlock.Text = mainText;
        ExtraTextBlock.Text = extraText;
        BottomTextBlock.Text = bottomText;
        
        Loaded += YesNoWindow_Loaded;
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    
    private void yesButton_Click(object sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }
    private void noButton_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
    
    public static bool Show(string mainText, string yesText, string noText, string extraText = "", string bottomText = "")
    {
        var messageBox = new YesNoMessagebox(mainText,yesText, noText, extraText, bottomText);
        messageBox.ShowDialog();
        return messageBox.Result;
    }
    
    private void YesNoWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ViewUtils.GetLayout().DisableEverything();
    }
    
    protected override void OnClosed(EventArgs e)
    {
        ViewUtils.GetLayout().EnableEverything();
        base.OnClosed(e);
    }
}
