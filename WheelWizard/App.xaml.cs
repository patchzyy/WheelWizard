using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.UrlProtocol;
using CT_MKWII_WPF.Views.Popups;
using System;
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
                HandleWheelWizardUrl(protocolArgument);
            }
        }
    }
    
    private void HandleWheelWizardUrl(string url)
    {
        try
        {
            // Remove the protocol prefix
            string content = url.Replace("wheelwizard://", "").Trim();
            // Remove any trailing slash
            content = content.TrimEnd('/');

            // Parse ModID and DownloadURL
            string[] parts = content.Split(',');

            if (!int.TryParse(parts[0], out int modID))
            {
                throw new FormatException($"Invalid ModID: {parts[0]}");
            }
            string downloadURL = parts.Length > 1 ? parts[1] : null;
            var modPopup = new ModIndependentPopup();
            // modPopup.LoadModAsync(modID, downloadURL);
            // modPopup.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error handling URL: {ex.Message}", "WheelWizard Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
}

