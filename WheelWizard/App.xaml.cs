using CT_MKWII_WPF.Services.Installation;
using System.Diagnostics;
using System.IO;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

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
