using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using WheelWizard.WPFViews.Popups.Generic;

namespace WheelWizard.Views.Popups.Generic;

public partial class TextInputWindow : PopupContent
{
    private string? _result;

    // Constructor with dynamic label parameter
    public TextInputWindow() : base(true, false, true, "Text Field")
    {
        InitializeComponent();
   
        InputField.TextChanged += InputField_TextChanged;
        UpdateSubmitButtonState();
    }

    public TextInputWindow setLabelText(string labelText)
    {
        DynamicLabel.Text = labelText;
        return this;
    }
    
    // Returns the entered text if "Submit" is clicked, or null if "Cancel" is clicked
    public new string? ShowDialog()
    {
        base.ShowDialog();
        return _result;
    }

    // Handle text changes to enable/disable Submit button
    private void InputField_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateSubmitButtonState();
    }

    // Update the Submit button's IsEnabled property based on input
    private void UpdateSubmitButtonState()
    {
        SubmitButton.IsEnabled = !string.IsNullOrWhiteSpace(InputField.Text);
    }

    // Handle Submit button click
    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        _result = InputField.Text.Trim();
        Close();
    }

    // Handle Cancel button click
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _result = null;
        Close();
    }
    
    public void PopulateText(string text)
    {
        InputField.Text = text;
    }
}

