using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes; 

namespace CT_MKWII_WPF.Views.Components
{
    public partial class ToolTipMessage : ToolTip
    {
        public enum ToolTipAlignment
        {
            TopLeft,
            TopCenter,
            TopRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
            Mouse
        }

        public ToolTipMessage()
        {
            InitializeComponent();
            CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PlaceToolTip);
            Opened += OnToolTipOpened;
        }

        
        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register(nameof(Alignment), typeof(ToolTipAlignment), typeof(ToolTipMessage), 
                new PropertyMetadata(ToolTipAlignment.TopCenter));
        
        public ToolTipAlignment Alignment
        {
            get => (ToolTipAlignment)GetValue(AlignmentProperty);
            set => SetValue(AlignmentProperty, value);
        }
              
        // Accessor for UpArrow
        private Path UpArrow => (Template.FindName("UpArrow", this) as Path)!;

        // Accessor for DownArrow
        private Path DownArrow => (Template.FindName("DownArrow", this) as Path)!;

        private void OnToolTipOpened(object sender, RoutedEventArgs e) => UpdatePlacement();
        private void UpdatePlacement()
        {
            HorizontalOffset = 0;
            VerticalOffset = 0;
            
            UpArrow.Visibility = Visibility.Collapsed;
            DownArrow.Visibility = Visibility.Collapsed;

            if (Alignment == ToolTipAlignment.Mouse)
            {
                Placement = PlacementMode.Mouse;
                return;
            }
            
            Placement = PlacementMode.Custom;
           
            if (Alignment is ToolTipAlignment.TopCenter or ToolTipAlignment.TopLeft or ToolTipAlignment.TopRight) 
                DownArrow.Visibility = Visibility.Visible;
            else // All Bottom sides
                UpArrow.Visibility = Visibility.Visible;   
            
            if (Alignment is ToolTipAlignment.TopLeft or ToolTipAlignment.BottomLeft)
                UpArrow.HorizontalAlignment = DownArrow.HorizontalAlignment = HorizontalAlignment.Left;
            else if (Alignment is ToolTipAlignment.TopRight or ToolTipAlignment.BottomRight)
                UpArrow.HorizontalAlignment = DownArrow.HorizontalAlignment = HorizontalAlignment.Right;
            else
                UpArrow.HorizontalAlignment = DownArrow.HorizontalAlignment = HorizontalAlignment.Center;
        }

        private CustomPopupPlacement[] PlaceToolTip(Size popupSize, Size targetSize, Point offset)
        {
            var axis = PopupPrimaryAxis.Horizontal;
            if (Alignment == ToolTipAlignment.Mouse)
                axis = PopupPrimaryAxis.Vertical;

            double offsetX = 0;
            double offsetY = 0;
                
            // IMPORTANT: The arrow has a margin of 15 on both sides, and it itself is 20 wide (so in total 50)
            
            var arrowNeedsCenter = targetSize.Width <= 70; 
            // This is just a threshold at this point its a bit ugly to have the left and right
            // be on literally the left and right of the object
            
            if (Alignment is ToolTipAlignment.BottomCenter or ToolTipAlignment.TopCenter)
                offsetX = (targetSize.Width - popupSize.Width) /2;
            else if (Alignment is ToolTipAlignment.BottomLeft or ToolTipAlignment.TopLeft)
                offsetX = arrowNeedsCenter ? targetSize.Width/2 - 25 : -10;
            else if (Alignment is ToolTipAlignment.BottomRight or ToolTipAlignment.TopRight)
                offsetX = -popupSize.Width + (arrowNeedsCenter ? targetSize.Width/2 + 25 : targetSize.Width + 10);
            
            if (Alignment is ToolTipAlignment.BottomCenter or ToolTipAlignment.BottomLeft or 
                ToolTipAlignment.BottomRight)
                offsetY = targetSize.Height;
            else
                offsetY = -popupSize.Height;
            
            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement(new Point(offsetX, offsetY), axis)
            };
        }
    }
}
