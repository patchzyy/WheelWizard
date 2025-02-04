using Avalonia.Controls;
using System;
using WheelWizard.Services;
using WheelWizard.Services.UrlProtocol;
using WheelWizard.Views.Pages.Settings;
using WheelWizard.Views.Popups.Generic;

namespace WheelWizard.Views;

public class ViewUtils
{
    public static void OpenLink(string link)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = link,
            UseShellExecute = true
        });
    }
    
    public static void OnInitialized(object? sender, EventArgs e)
    {
        var args = Environment.GetCommandLineArgs();
        ModManager.Instance.ReloadAsync();
        if (args.Length <= 1) return; 
        var protocolArgument = args[1];
        UrlProtocolManager.ShowPopupForLaunchUrlAsync(protocolArgument);
    }
    
    public static Layout GetLayout() => Layout.Instance;
    public static void NavigateToPage(UserControl page) => GetLayout().NavigateToPage(page);

    public static void RefreshWindow(SettingsPage settingsPage)
    {
       
    }
}
