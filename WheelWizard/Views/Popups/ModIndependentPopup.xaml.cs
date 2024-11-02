// ModIndependentPopup.xaml.cs
using System.Threading.Tasks;
using System.Windows;
using CT_MKWII_WPF.Services.GameBanana;
using CT_MKWII_WPF.Views.Components;

namespace CT_MKWII_WPF.Views.Popups
{
    public partial class ModIndependentPopup : PopupContent
    {
        public ModIndependentPopup(Window owner = null) 
            : base(true, true, true, "Mod Details", new Vector(400, 600), owner)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the specified mod details into the ModDetailViewer.
        /// </summary>
        /// <param name="mod">The mod to display.</param>
        public async Task LoadModAsync(ModRecord mod)
        {
            if (mod != null)
            {
                await ModDetailViewer.LoadModDetailsAsync(mod);
            }
            else
            {
                MessageBox.Show("Invalid mod record provided.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
