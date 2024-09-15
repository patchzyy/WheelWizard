using System.Windows;

namespace CT_MKWII_WPF.Views.Popups;

public partial class YesNoWindow : PopupContent
{
    public bool Result { get; private set; }
    
    public YesNoWindow() : base(false, false, "Wheel Wizard")
    {
        InitializeComponent();
        YesButton.Text = "Yes";
        NoButton.Text = "No";
    }
    
    public YesNoWindow SetMainText(string mainText)
    {
        MainTextBlock.Text = mainText;
        return this;
    }
    
    public YesNoWindow SetExtraText(string extraText)
    {
        ExtraTextBlock.Text = extraText;
        return this;
    }
    
    public YesNoWindow SetButtonText(string yesText, string noText)
    {
        YesButton.Text = yesText;
        NoButton.Text = noText;
        
        // It really depends on the text length what looks best
        ButtonContainer.HorizontalAlignment = (yesText.Length + noText.Length) > 12
            ? HorizontalAlignment.Stretch : HorizontalAlignment.Right;
        return this;
    }
    
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
    
    public bool AwaitAnswer()
    {
        ShowDialog();
        return Result;
    }
}
