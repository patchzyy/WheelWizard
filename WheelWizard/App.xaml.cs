using System.Windows;
using CT_MKWII_WPF.Utilities.Auto_updator;

namespace CT_MKWII_WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            VersionChecker.CheckForUpdates();
        }
    }
}