using System.Windows;

using System.Windows.Media;

/*
 EXAMPLES:                 
<local:IconLabelButton IconKind="{x:Static icon:PackIconMaterialKind.Account}" 
                 IconPack="Material"
                 Text="User Profile" 
                 Color="Blue" 
                 FontSize="20"
                 IconSize="24"
                 HoverColor="Red"
                 Click="YourClickHandler"/>
 */

namespace CT_MKWII_WPF.Views.Components
{
    public partial class IconLabelButton : System.Windows.Controls.Button
    {
        public IconLabelButton()
        {
            FontSize = 16;
            InitializeComponent();
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Brush), typeof(IconLabelButton), 
            new PropertyMetadata(Brushes.Black));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty HoverColorProperty = DependencyProperty.Register(
            nameof(HoverColor), typeof(Brush), typeof(IconLabelButton),
            new PropertyMetadata(null));

        public Brush HoverColor
        {
            get => (Brush)GetValue(HoverColorProperty);
            set => SetValue(HoverColorProperty, value);
        }
        
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
            nameof(IconSize), typeof(double), typeof(IconLabelButton),
            new PropertyMetadata(20.0));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(IconLabelButton), 
            new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register(
            nameof(IconKind), typeof(object), typeof(IconLabelButton), 
            new PropertyMetadata(null));

        public object IconKind
        {
            get => GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public static readonly DependencyProperty IconPackProperty = DependencyProperty.Register(
            nameof(IconPack), typeof(string), typeof(IconLabelButton), 
            new PropertyMetadata(null));

        public string IconPack
        {
            get => (string)GetValue(IconPackProperty);
            set => SetValue(IconPackProperty, value);
        }
    }
}
