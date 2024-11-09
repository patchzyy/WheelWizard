using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.UrlProtocol;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SettingsManager.Instance.LoadSettings();
        AutoUpdater.CheckForUpdatesAsync();
        UrlProtocolManager.SetWhWzSchemeAsync();
        // Parse command-line arguments
        string[] args = Environment.GetCommandLineArgs();

        if (args.Length > 1) // The first argument is always the executable path
        {
            string protocolArgument = args[1];
            if (protocolArgument.StartsWith("wheelwizard://", StringComparison.OrdinalIgnoreCase))
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    await Task.Delay(1);
                    UrlProtocolManager.ShowPopupForLaunchUrlAsync(protocolArgument);
                });
            }
        }
    }
    
}

