using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views;

public partial class ProgressWindow : Window
{
    public ProgressWindow()
    {
        InitializeComponent();
    }

    public void UpdateProgress(int progress, string status, string bottomText)
    {
        ProgressBar.Value = progress;
        StatusLabel.Text = status;
        BottomTextLabel.Text = bottomText;
    }
    
    public void ChangeExtraText(string text) =>  ExtraTextLabel.Text = text;
    
    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}