using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class DynamicIcon : UserControl
    {
        public static readonly DependencyProperty IconPackProperty =
            DependencyProperty.Register("IconPack", typeof(string), typeof(DynamicIcon), 
                new PropertyMetadata("Material"));

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register("IconKind", typeof(object), typeof(DynamicIcon), 
                new PropertyMetadata(null));

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(DynamicIcon),
                new PropertyMetadata(16.0));

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Brush), typeof(DynamicIcon),
                new PropertyMetadata(Brushes.Black));

        public string IconPack
        {
            get { return (string)GetValue(IconPackProperty); }
            set { SetValue(IconPackProperty, value); }
        }

        public object IconKind
        {
            get { return GetValue(IconKindProperty); }
            set { SetValue(IconKindProperty, value); }
        }

        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public DynamicIcon()
        {
            InitializeComponent();
        }
    }
}
