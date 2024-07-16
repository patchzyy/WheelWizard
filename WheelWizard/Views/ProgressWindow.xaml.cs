using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Pages;

public partial class ProgressWindow : Window
{
    public ProgressWindow()
    {
        InitializeComponent();
    }

    public void UpdateProgress(int progress, string status, string extratext = "", string BottomText = "")
    {
        ProgressBar.Value = progress;
        StatusLabel.Text = status;
        ExtraTextLabel.Text = extratext;
        BottomTextLabel.Text = BottomText;
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}