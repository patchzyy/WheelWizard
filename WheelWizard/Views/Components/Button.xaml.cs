using System.Windows;
/*
EXAMPLES:

# all parameters
 <components:Button Variant="Primary"
                    IsEnabled="True"
                    IconPack="FontAwesome"
                    IconKind="{x:Static icon:PackIconFontAwesomeKind.PlaySolid}"
                    IconSize="20"
                    Text="Home"
                    FontSize="16"
                    Click="Button_OnClick"/>

# but you dont need all
 <components:Button IconSize="0"
                    Text="Home"
                    Click="Button_OnClick"/>
                    
# Note1:  you either need to find the icon, or set IconSize to 0 to hide it
 */

namespace CT_MKWII_WPF.Views.Components
{
    public partial class Button : System.Windows.Controls.Button
    {
        public Button()
        {
            InitializeComponent();
            Style = (Style)Application.Current.FindResource("DefaultButtonStyle")!;
            FontSize = 14.0;
        }
        
        public enum ButtonsVariantType
        {
            Primary,
            Secondary,
            Default,
            Danger
        }

        public static readonly DependencyProperty VariantProperty =
            DependencyProperty.Register(nameof(Variant), typeof(ButtonsVariantType), typeof(Button), 
                new PropertyMetadata(ButtonsVariantType.Default, OnVariantChanged));
        
        public ButtonsVariantType Variant
        {
            get => (ButtonsVariantType)GetValue(VariantProperty);
            set => SetValue(VariantProperty, value);
        }
        
        private static void OnVariantChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Button button) return;
            var type = (ButtonsVariantType)e.NewValue;
            var styleName = type switch
            {
                ButtonsVariantType.Primary => "PrimaryButtonStyle",
                ButtonsVariantType.Secondary => "SecondaryButtonStyle",
                ButtonsVariantType.Danger => "DangerButtonStyle",
                _ => "DefaultButtonStyle"
            };
                
            button.Style = (Style)Application.Current.FindResource(styleName)!;
        }

        public static readonly DependencyProperty IconPackProperty =
            DependencyProperty.Register(nameof(IconPack), typeof(string), typeof(Button),
                new PropertyMetadata(null));

        public string IconPack
        {
            get => (string)GetValue(IconPackProperty);
            set => SetValue(IconPackProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(object), typeof(Button),
                new PropertyMetadata(null));
            
        public object IconKind
        {
            get => GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }
        
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(Button), 
                new PropertyMetadata(20.0));
        
        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Button), 
                new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
