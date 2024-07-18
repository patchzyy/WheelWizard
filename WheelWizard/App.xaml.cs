using System.Windows;
using CT_MKWII_WPF.Services;

namespace CT_MKWII_WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AutoUpdater.CheckForUpdatesAsync();
        }
    }
}