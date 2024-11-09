// ModIndependentPopup.xaml.cs

using CT_MKWII_WPF.Models.GameBanana;
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Services.GameBanana;
using CT_MKWII_WPF.Views.Components;
using System.Linq;
using static CT_MKWII_WPF.Models.GameBanana.ModDetailResponse;

namespace CT_MKWII_WPF.Views.Popups
{
    public partial class ModIndependentPopup : PopupContent
    {
        // (bool allowClose, bool allowLayoutInteraction,bool isTopMost, string title = "", Vector? size = null)
        public ModIndependentPopup(Window owner = null) : base(allowClose: true, allowLayoutInteraction: false, isTopMost: true, title: "Mod Details", owner: owner)
        {
            MessageBox.Show("This is a test message box", "Test", MessageBoxButton.OK, MessageBoxImage.Information);
            InitializeComponent();
            MessageBox.Show("This is a test message box2", "Test", MessageBoxButton.OK, MessageBoxImage.Information);
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
}
