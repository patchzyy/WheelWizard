using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF
{
    public partial class NANDTutorialWindow : Window
    {

        public NANDTutorialWindow()
        {
            InitializeComponent();
        }

        private void OpenLink1(object sender, RoutedEventArgs e)
        {
            OpenLink("https://www.youtube.com/watch?v=LwJsXx7px-Y");
        }

        private void OpenLink2(object sender, RoutedEventArgs e)
        {
            OpenLink("https://www.youtube.com/watch?v=WHjhrpzXb6g");
        }

        private void OpenLink(string link)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error opening link: {ex.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

}