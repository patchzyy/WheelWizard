using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace WheelWizard.Views.Components
{
    public partial class DynamicIcon : UserControl
    {
        public DynamicIcon()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<Geometry> IconDataProperty =
            AvaloniaProperty.Register<DynamicIcon, Geometry>(nameof(IconData));

        public Geometry IconData
        {
            get => GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public static readonly StyledProperty<double> IconSizeProperty =
            AvaloniaProperty.Register<DynamicIcon, double>(nameof(IconSize), 20.0); // Added a default value

        public static readonly StyledProperty<IBrush> ForegroundColorProperty =
            AvaloniaProperty.Register<DynamicIcon, IBrush>(nameof(ForegroundColor), Brushes.Black); // Default to Black

        public double IconSize
        {
            get => GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public IBrush ForegroundColor
        {
            get => GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }
    }
}
