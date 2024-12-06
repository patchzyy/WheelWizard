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
    
    //public static Layout GetLayout() => (Layout)Application.Current.MainWindow!;
    //public static void NavigateToPage(Page page) => GetLayout().NavigateToPage(page);
}
