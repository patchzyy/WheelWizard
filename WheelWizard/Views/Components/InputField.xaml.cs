using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class InputField : TextBox
    {
        public InputField()
        {
            InitializeComponent();
            FontSize = 16;
            Style = (Style)FindResource("DefaultVariant")!;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateLabelVisibility();
        }
        
        public enum InputVariantType
        {
            Default,
            Dark
        }

        public static readonly DependencyProperty VariantProperty =
            DependencyProperty.Register(nameof(Variant), typeof(InputVariantType), typeof(InputField), 
                new PropertyMetadata(InputVariantType.Default, OnVariantChanged));
        
        public InputVariantType Variant
        {
            get => (InputVariantType)GetValue(VariantProperty);
            set => SetValue(VariantProperty, value);
        }
        
        private static void OnVariantChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InputField field)
            {
                var type = (InputVariantType)e.NewValue;
                string styleName = type switch
                {
                    InputVariantType.Dark => "DarkVariant",
                    _ => "DefaultVariant"
                };
                
                field.Style = (Style)field.FindResource(styleName)!;
            }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(InputField), 
                new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }
        
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(InputField), 
                new PropertyMetadata(string.Empty, OnLabelChanged));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
        
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InputField inputField)
                inputField.UpdateLabelVisibility();
        }
        
        private void UpdateLabelVisibility()
        {
            if (Template.FindName("LabelElement", this) is FormFieldLabel labelElement)
                labelElement.Visibility = string.IsNullOrEmpty(Label) ? Visibility.Collapsed : Visibility.Visible;
        }
        
        public static readonly DependencyProperty LabelTipProperty =
            DependencyProperty.Register(nameof(LabelTip), typeof(string), typeof(InputField), 
                new PropertyMetadata(string.Empty));

        public string LabelTip
        {
            get => (string)GetValue(LabelTipProperty);
            set => SetValue(LabelTipProperty, value);
        }
    }
}