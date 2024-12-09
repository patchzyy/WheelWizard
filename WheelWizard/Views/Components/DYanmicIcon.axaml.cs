using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Data;

namespace WheelWizard.Views.Components
{
    public partial class DynamicIcon : UserControl
    {
        public DynamicIcon()
        {
            InitializeComponent();
        }
        public static readonly StyledProperty<IImage?> IconDataProperty = AvaloniaProperty.Register<DynamicIcon, IImage?>(nameof(IconData));
        public static readonly StyledProperty<double> IconSizeProperty = AvaloniaProperty.Register<DynamicIcon, double>(nameof(IconSize));
        public static readonly StyledProperty<IBrush> ForegroundColorProperty = AvaloniaProperty.Register<DynamicIcon, IBrush>(nameof(ForegroundColor), Brushes.Black);

        public IImage? IconData
        {
            get => GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

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
