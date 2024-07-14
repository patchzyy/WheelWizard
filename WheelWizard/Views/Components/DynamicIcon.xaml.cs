using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class DynamicIcon : UserControl
    {
        public DynamicIcon() =>  InitializeComponent();
        
        public static readonly DependencyProperty IconPackProperty =
            DependencyProperty.Register(nameof(IconPack), typeof(string), typeof(DynamicIcon), 
                new PropertyMetadata("Material"));

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(object), typeof(DynamicIcon), 
                new PropertyMetadata(null));

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(DynamicIcon),
                new PropertyMetadata(16.0));

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(nameof(ForegroundColor), typeof(Brush), typeof(DynamicIcon),
                new PropertyMetadata(Brushes.Black));

        public string IconPack
        {
            get => (string)GetValue(IconPackProperty);
            set => SetValue(IconPackProperty, value);
        }

        public object IconKind
        {
            get => GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public Brush ForegroundColor
        {
            get => (Brush)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }
    }
}
