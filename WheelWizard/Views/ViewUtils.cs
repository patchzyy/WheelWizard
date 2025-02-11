using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using System;
using System.Globalization;
using WheelWizard.Services;
using WheelWizard.Services.LiveData;
using WheelWizard.Services.Settings;
using WheelWizard.Services.UrlProtocol;
using WheelWizard.Services.WiiManagement.SaveData;
using WheelWizard.Utilities.RepeatedTasks;
using WheelWizard.Views.Pages;
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
    
    public static void OnInitialized(object? sender, EventArgs e)
    {
        var args = Environment.GetCommandLineArgs();
        ModManager.Instance.ReloadAsync();
        if (args.Length <= 1) return; 
        var protocolArgument = args[1];
        UrlProtocolManager.ShowPopupForLaunchUrlAsync(protocolArgument);
    }
    
    public static Layout GetLayout() => Layout.Instance;
    public static void NavigateToPage(UserControl page)
    {
        // TODO: Fix the langauge bug. for some reason when changing the language, it changes itself back to the language before
        //  SO as a quick and dirty fix in the navigate to page we just set the langauge pack when its out of sinc, but this solution
        //  still makes it so that the first page you enter after changing the language setting will always be the old langauge instead of the new one
        //  when working on the translations again, this should be fixed. and in a solid way instead of this
        var itCurrentlyIs = CultureInfo.CurrentCulture.ToString();
        var itsSupposeToBe = (string)SettingsManager.WW_LANGUAGE.Get();
        if (itCurrentlyIs != itsSupposeToBe)
        {
            SettingsManager.WW_LANGUAGE.Set(itCurrentlyIs);
            SettingsManager.WW_LANGUAGE.Set(itsSupposeToBe);
        }
        GetLayout().NavigateToPage(page);
    }

    public static void RefreshWindow()
    {
        // Refresh window  opens in the start page again, that is nessesairy
        // we would prefer opening up where we left off, however, that does not work since the translations
        // are still in the context of the layout before, and so the dropdowns will break
        
        var oldWindow = GetLayout();
        // Creating a new one will also set re-assign `Layout.Instance` right away, and this `GetLayout()`
        Layout newWindow = new();
        newWindow.Position = oldWindow.Position;
        if (oldWindow is IRepeatedTaskListener oldListener)
        {
            // Unsubscribing is not really necessary. But i guess it prevents memory leaks when
            // someone is refreshing the window a lot (happens when changing the language e.g.
            // So they would have to change the language like 1000 of times in a row)
            LiveAlertsManager.Instance.Unsubscribe(oldListener);
            RRLiveRooms.Instance.Unsubscribe(oldListener);
            GameDataLoader.Instance.Unsubscribe(oldListener);
        }
     
        newWindow.Show();
        oldWindow.Close();
        newWindow.UpdatePlayerAndRoomCount(RRLiveRooms.Instance);
    }

    public static T? FindParent<T>(object? child, int maxSearchDepth = 10)
    {
        StyledElement? currentParent = null;
        if (child is StyledElement childElement) currentParent = childElement.Parent;
        if (currentParent == null) return default;
        
        var currentDepth = 1;
        while (currentDepth < maxSearchDepth)
        {
            if (currentParent is T parentElement) return parentElement;
            if (currentParent?.Parent != null) currentParent = currentParent.Parent;
            currentDepth++;
        }
        return default;
    }
    
}

