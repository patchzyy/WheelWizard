using System;
using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Pages.Popups;

public partial class YesNoMessagebox : Window
{
    public bool Result { get; private set; }
    
    public YesNoMessagebox(string mainText, string yesText, string noText, string extraText = "", string bottomText = "")
    {
        InitializeComponent();
        ViewUtils.GetLayout().DisableEverything();
        YesButton.Text = yesText;
        NoButton.Text = noText;
        MainTextBlock.Text = mainText;
        ExtraTextBlock.Text = extraText;
        BottomTextBlock.Text = bottomText;
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
    
    protected override void OnClosed(EventArgs e)
    {
        ViewUtils.GetLayout().EnableEverything();
        base.OnClosed(e);
    }
    
}


