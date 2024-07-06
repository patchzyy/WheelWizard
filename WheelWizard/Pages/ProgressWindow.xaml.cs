using System.Windows;

namespace CT_MKWII_WPF.Pages;

public partial class ProgressWindow : Window
{
    public ProgressWindow()
    {
        InitializeComponent();
    }

    public void UpdateProgress(int progress, string status)
    {
        ProgressBar.Value = progress;
        StatusLabel.Content = status;
    }
    
}