// ModIndependentPopup.xaml.cs
using System.Threading.Tasks;
using System.Windows;

namespace CT_MKWII_WPF.Views.Popups;
public partial class ModIndependentPopup : PopupContent
{
    public ModIndependentPopup(Window owner = null) : base(allowClose: true, allowLayoutInteraction: false, isTopMost: true, title: "Mod Details", owner: owner, size: new Vector(500, 700))
    {
        InitializeComponent();
    }
    /// <summary>
    /// Loads the specified mod details into the ModDetailViewer.
    /// </summary>
    /// <param name="mod">The mod to display.</param>
    public async Task LoadModAsync(int modID, string NewDownloadURL = null)
    {
        await ModDetailViewer.LoadModDetailsAsync(modID, NewDownloadURL);
    }
}

