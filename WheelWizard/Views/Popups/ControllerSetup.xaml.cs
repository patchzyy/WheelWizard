using System.Windows;

namespace CT_MKWII_WPF.Views.Popups;

public partial class ControllerSetup : PopupContent
{
    public ControllerSetup() : base(allowClose: true, allowLayoutInteraction: false, isTopMost: false, title: "Controller Setup", size: new Vector(500,800))
    {
        InitializeComponent();
    }
}

