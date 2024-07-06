using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CT_MKWII_WPF.Pages;
using CT_MKWII_WPF.Pages.WiiMods;
using CT_MKWII_WPF.Utils;

namespace CT_MKWII_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ChangeContent(new SettingsPage());
            SettingsButton.Opacity = 0.5;
        }
        
        public void ChangeContent(UserControl control)
        {
            ContentArea.Content = control;
        }
        public void SwitchContent()
        {
            if (!SettingsUtils.doesConfigExist())
            {
                MessageBox.Show("Please set the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ChangeContent(new SettingsPage());
                return;
            }
            // Handle button clicks and switch content accordingly
        }
        
        private void resetAllOpacity(Button currentlyClickedButton)
        {
            SettingsButton.Opacity = 1;
            MyStuffButton.Opacity = 1;
            GameButton.Opacity = 1;
            // ExtrasButton.Opacity = 1;
            currentlyClickedButton.Opacity = 0.5;
        }
        

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ChangeContent(new SettingsPage());
            resetAllOpacity((Button)sender);
        }
        

        private void MyStuff_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsUtils.IsConfigFileFinishedSettingUp())
            {
                MessageBox.Show("Please set all the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ChangeContent(new SettingsPage());
                return;
            }
            ChangeContent(new MyStuffManager());
            resetAllOpacity((Button)sender);
        }

        private void Game_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsUtils.IsConfigFileFinishedSettingUp())
            {
                MessageBox.Show("Please set all the paths in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ChangeContent(new SettingsPage());
                return;
            }
            ChangeContent(new RetroRewindDolphin());
            resetAllOpacity((Button)sender);
        }

        private void Extras_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature is not yet implemented.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            resetAllOpacity((Button)sender);
        }
        
        
    }
}