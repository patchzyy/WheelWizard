using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views;

public partial class ProgressWindow : Window
{
    public ProgressWindow()
    {
        InitializeComponent();
    }

    public void UpdateProgress(int progress, string status, string extratext = "", string bottomText = "")
    {
        ProgressBar.Value = progress;
        StatusLabel.Text = status;
        ExtraTextLabel.Text = extratext;
        BottomTextLabel.Text = bottomText;
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}