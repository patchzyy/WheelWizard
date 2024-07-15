using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class StateBox : UserControl
    {
        public StateBox()
        {
            InitializeComponent();
            FontSize = 14;
            StateBoxToolTip.Visibility = string.IsNullOrEmpty(TipText) ? Visibility.Hidden : Visibility.Visible;
        }
        
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(StateBox), 
                new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        
        public static readonly DependencyProperty IsDarkProperty =
            DependencyProperty.Register(nameof(IsDark), typeof(bool), typeof(StateBox), 
                new PropertyMetadata(false));

        public bool IsDark
        {
            get => (bool)GetValue(IsDarkProperty);
            set => SetValue(IsDarkProperty, value);
        }
        
        public static readonly DependencyProperty IconPackProperty =
            DependencyProperty.Register(nameof(IconPack), typeof(string), typeof(StateBox),
            new PropertyMetadata("Material"));

        public string IconPack
        {
            get => (string)GetValue(IconPackProperty);
            set => SetValue(IconPackProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(object), typeof(StateBox), 
                new PropertyMetadata(null));
        
        public object IconKind
        {
            get => GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(StateBox),
                new PropertyMetadata(20.0));
        
        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty TipAlignmentProperty =
            DependencyProperty.Register(nameof(TipAlignment), typeof(ToolTipMessage.ToolTipAlignment),
                typeof(StateBox), 
                new PropertyMetadata(ToolTipMessage.ToolTipAlignment.TopCenter, OnTipAlignmentChanged));

        public ToolTipMessage.ToolTipAlignment TipAlignment
        {
            get => (ToolTipMessage.ToolTipAlignment)GetValue(TipAlignmentProperty);
            set => SetValue(TipAlignmentProperty, value);
        }
        
        private static void OnTipAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // quick and dirty fix to set the tooltip, because for some reason
            // `Content="{Binding Text, ElementName=Root}"` seems to not work, who knows why ¯\_(ツ)_/¯
            if (d is StateBox formFieldLabel)
                formFieldLabel.StateBoxToolTip.Alignment = (ToolTipMessage.ToolTipAlignment)e.NewValue;
        }
        
        public static readonly DependencyProperty TipTextProperty =
            DependencyProperty.Register(nameof(TipText), typeof(string), typeof(StateBox), 
                new PropertyMetadata(string.Empty, OnTipTextChanged));
        
        public string TipText
        {
            get => (string)GetValue(TipTextProperty);
            set => SetValue(TipTextProperty, value);
        }
        
        private static void OnTipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // quick and dirty fix to set the tooltip, because for some reason
            // `Content="{Binding Text, ElementName=Root}"` seems to not work, who knows why ¯\_(ツ)_/¯
            if (d is not StateBox formFieldLabel) return;
            var newTip = (string)e.NewValue;
            var tipElement = formFieldLabel.StateBoxToolTip;
            tipElement.Content = newTip;
            tipElement.Visibility = string.IsNullOrEmpty(newTip) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
