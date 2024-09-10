using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Popups;

public abstract class PopupContent : UserControl
{
    public PopupWindow Window { get; private set; }
    
    protected PopupContent( bool allowClose = true, bool allowLayoutInteraction = false)
    {
        Window = new(allowClose, allowLayoutInteraction ) { PopupContent = { Content = this } };
    }

    public void Show() => Window.Show();
    public bool? ShowDialog() => Window.ShowDialog();
    public void Close() => Window.Close();
    public void Minimize() => Window.WindowState = System.Windows.WindowState.Minimized;
    public new void Focus() => Window.Focus();
}
