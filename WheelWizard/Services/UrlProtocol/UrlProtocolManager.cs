using CT_MKWII_WPF.Views.Popups;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CT_MKWII_WPF.Services.UrlProtocol;

public class UrlProtocolManager
{
    public const string ProtocolName = "wheelwizard";
    public static void RegisterCustomScheme(string schemeName)
    {
        var currentExecutablePath = Process.GetCurrentProcess().MainModule!.FileName;
        var protocolKey = $@"SOFTWARE\Classes\{schemeName}";

        using var key = Registry.CurrentUser.CreateSubKey(protocolKey);

        key.SetValue("", $"URL:{schemeName} Protocol");
        key.SetValue("URL Protocol", "");


        using var shellKey = key.CreateSubKey(@"shell\open\command");
        shellKey.SetValue("", $"\"{currentExecutablePath}\" \"%1\"");
    }
    
    public static bool IsCustomSchemeRegistered(string schemeName)
    {
        var protocolKey = $@"SOFTWARE\Classes\{schemeName}";
        return Registry.CurrentUser.OpenSubKey(protocolKey) != null;
    }

    async public static void SetWhWzSchemeAsync()
    {
        var currentExecutablePath = Process.GetCurrentProcess().MainModule!.FileName;
        var protocolKey = $@"SOFTWARE\Classes\{ProtocolName}";

        // Check if the scheme is registered
        using var key = Registry.CurrentUser.OpenSubKey(protocolKey);
        if (key == null)
        {
            RegisterCustomScheme(ProtocolName);
            return;
        }
        
        using var shellKey = key.OpenSubKey(@"shell\open\command");
        if (shellKey != null)
        {
            var registeredExecutablePath = shellKey.GetValue("") as string;

            if (registeredExecutablePath != null)
            {
                // Extract the path from the registered string (which might have quotes and "%1")
                registeredExecutablePath = registeredExecutablePath.Split('\"')[1];

                // If the registered executable is different from the current one, repair it
                if (!registeredExecutablePath.Equals(currentExecutablePath, StringComparison.OrdinalIgnoreCase))
                {
                    // Fix the scheme by re-registering with the current executable
                    RegisterCustomScheme(ProtocolName);
                }
            }
        }
    }
    
    public static void RemoveCustomScheme(string schemeName)
    {
        var protocolKey = $@"SOFTWARE\Classes\{schemeName}";
        Registry.CurrentUser.DeleteSubKeyTree(protocolKey);
    }

    public static void ShowPopupForLaunchUrl(string url)
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
            modPopup.LoadModAsync(1234);
            modPopup.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error handling URL: {ex.Message}", "WheelWizard Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static async Task ShowPopupForLaunchUrlAsync(string url)
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
            MessageBox.Show($"ModID: {modID}, DownloadURL: {downloadURL}", "WheelWizard URL", MessageBoxButton.OK, MessageBoxImage.Information);
    
            //sleep for 2 seconds
            var modPopup = new ModIndependentPopup();
            await modPopup.LoadModAsync(modID, downloadURL);
            modPopup.ShowDialog();

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error handling URL: {ex.Message}", "WheelWizard Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

