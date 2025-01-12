using Avalonia;
using Avalonia.Controls;
using System.Threading.Tasks;
using WheelWizard.Views.Pages;

namespace WheelWizard.Views.Popups.ModManagement;

public partial class ModIndependentWindow : PopupContent
{
    public ModIndependentWindow(Window owner = null) :  base(true, false, true, "Mod Details", new Vector(500, 700))
    {
        InitializeComponent();
    }
    
    public async Task LoadModAsync(int modId, string newDownloadUrl = null)
    {
        await ModDetailViewer.LoadModDetailsAsync(modId, newDownloadUrl);
    }
        
    protected override void BeforeClose()
    {
        ViewUtils.NavigateToPage(new ModsPage());
        base.BeforeClose();
    }
}
