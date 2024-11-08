using System.Media;
using System.Windows;

namespace CT_MKWII_WPF.Views.Popups
{
    public partial class ErrorMessageWindow : PopupContent
    {
        private ErrorMessageWindow(string errorMessage) : base(true, false, true, "Error")
        {
            InitializeComponent();
            ErrorTextBlock.Text = errorMessage;

            // Play error sound
            SystemSounds.Hand.Play();
        }

        public static void Show(string errorMessage)
        {
            var errorWindow = new ErrorMessageWindow(errorMessage);
            errorWindow.ShowDialog();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
