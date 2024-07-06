using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class InputField : UserControl
    {
        public InputField()
        {
            InitializeComponent();
            FontSize = 16;
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(InputField), 
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(InputField), 
                new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(InputField), 
                new PropertyMetadata(string.Empty));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        
        public static readonly DependencyProperty LabelTipProperty =
            DependencyProperty.Register(nameof(LabelTip), typeof(string), typeof(InputField), 
                new PropertyMetadata(string.Empty));

        public string LabelTip
        {
            get { return (string)GetValue(LabelTipProperty); }
            set { SetValue(LabelTipProperty, value); }
        }
    }
}