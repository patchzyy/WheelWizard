using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Popups
{
    public partial class TextInputPopup : PopupContent
    {
        private string? _result;

        // Constructor with dynamic label parameter
        public TextInputPopup(string labelText, Window owner = null)
            : base(allowClose: true, allowLayoutInteraction: false, isTopMost: true, title: "Text Field", owner: owner)
        {
            InitializeComponent();

            // Set the dynamic label text
            DynamicLabel.Text = labelText;

            // Subscribe to text changed event
            InputField.TextChanged += InputField_TextChanged;

            // Initialize Submit button state
            UpdateSubmitButtonState();
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
}
