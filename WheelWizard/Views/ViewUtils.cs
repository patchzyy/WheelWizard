using Avalonia.Controls;
using System;
using WheelWizard.Views.Pages.Settings;

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
    
    public static Layout GetLayout() => Layout.Instance;
    public static void NavigateToPage(UserControl page) => GetLayout().NavigateToPage(page);

    public static void RefreshWindow(SettingsPage settingsPage)
    {
        Console.WriteLine("boop");
    }
}
