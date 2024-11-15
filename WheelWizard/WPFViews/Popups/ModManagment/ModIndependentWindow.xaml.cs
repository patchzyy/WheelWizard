// ModIndependentPopup.xaml.cs

using System.Threading.Tasks;
using System.Windows;
using WheelWizard.Views.Pages;

namespace WheelWizard.Views.Popups;
public partial class ModIndependentWindow : PopupContent
{
    public ModIndependentWindow(Window owner = null) : base(allowClose: true, allowLayoutInteraction: false, isTopMost: true, title: "Mod Details", owner: owner, size: new Vector(500, 700))
    {
        InitializeComponent();
    }
    /// <summary>
    /// Loads the specified mod details into the ModDetailViewer.
    /// </summary>
    /// <param name="modId">The ID of the mod to load.</param>
    /// <param name="newDownloadUrl">The download URL to use instead of the one from the mod details.</param>
    public async Task LoadModAsync(int modId, string newDownloadUrl = null)
    {
        await ModDetailViewer.LoadModDetailsAsync(modId, newDownloadUrl);
    }
    
    protected override void BeforeClose()
    {
        // a bit dirty, but it's the easiest way to refresh the mod list in the ModsPage
        ViewUtils.NavigateToPage(new ModsPage());
        base.BeforeClose();
    }
}

