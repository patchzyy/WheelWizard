using CT_MKWII_WPF.Services.LiveData;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views;

using System.Windows;
using System.Windows.Media;

// when using any of the utils, import using:
//  using static CT_MKWII_WPF.Views.ViewUtils;

public static class ViewUtils
{
    // This method returns the first parent of the given type
    // or null if no parent of that type is found
    public static T? FindAncestor<T>(object child) where T : DependencyObject
    {
        // lazy me, but i dont want to constantly typeCast to DependencyObject when using this method,
        // so i just move it in to this method
        if (!(child is DependencyObject))
            return null;
        var current = child as DependencyObject;

        while (current != null)
        {
            if (current is T typedAncestor) return typedAncestor;
            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    public static Layout GetLayout() => (Layout)Application.Current.MainWindow!;
    public static void NavigateToPage(Page page) => GetLayout().NavigateToPage(page);

    public static void RefreshWindow(Page? destinationPage = null)
    {
        Layout newWindow = destinationPage == null ? new() : new(destinationPage);
        
        var oldWindow = Application.Current.MainWindow;
        Application.Current.MainWindow = newWindow;
        // Set position of new window to the position of the old window
        newWindow.Left = oldWindow!.Left;
        newWindow.Top = oldWindow.Top;
        newWindow.Show();
        oldWindow.Close();

        newWindow.UpdatePlayerAndRoomCount(RRLiveRooms.Instance);
    }
}
