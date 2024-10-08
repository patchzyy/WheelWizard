using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Popups;

public abstract class PopupContent : UserControl
{
    public PopupWindow Window { get; private set; }
    
    protected PopupContent( bool allowClose, bool allowLayoutInteraction, string title = "", Vector? size = null)
    {
        Window = new(allowClose, allowLayoutInteraction, title, size)
        {
            PopupContent = { Content = this },
            BeforeClose = BeforeClose,
            BeforeOpen = BeforeOpen
        };
    }

    protected virtual void BeforeClose() { } // Meant to be overwritten if needed
    protected virtual void BeforeOpen() { }  // Meant to be overwritten if needed
    
    public void Show() => Window.Show();
    public bool? ShowDialog() => Window.ShowDialog();
    public void Close() => Window.Close();
    public void Minimize() => Window.WindowState = System.Windows.WindowState.Minimized;
    public new void Focus() => Window.Focus();
}
