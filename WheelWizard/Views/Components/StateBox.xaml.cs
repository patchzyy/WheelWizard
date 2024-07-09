using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class StateBox : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(StateBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsDarkProperty =
            DependencyProperty.Register(nameof(IsDark), typeof(bool), typeof(StateBox), new PropertyMetadata(false));

        public static readonly DependencyProperty IconPackProperty =
            DependencyProperty.Register(nameof(IconPack), typeof(string), typeof(StateBox), new PropertyMetadata("Material"));

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(object), typeof(StateBox), new PropertyMetadata(null));

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(StateBox), new PropertyMetadata(20.0));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(StateBox), new PropertyMetadata(14.0));

        public static readonly DependencyProperty TipTextProperty =
            DependencyProperty.Register(nameof(TipText), typeof(string), typeof(StateBox), 
                new PropertyMetadata(string.Empty, OnTipTextChanged));

        public static readonly DependencyProperty TipAlignmentProperty =
            DependencyProperty.Register(nameof(TipAlignment), typeof(ToolTipMessage.ToolTipAlignment), typeof(StateBox), 
                new PropertyMetadata(ToolTipMessage.ToolTipAlignment.TopCenter, OnTipAlignmentChanged));

        public string TipText
        {
            get { return (string)GetValue(TipTextProperty); }
            set { SetValue(TipTextProperty, value); }
        }

        public ToolTipMessage.ToolTipAlignment TipAlignment
        {
            get { return (ToolTipMessage.ToolTipAlignment)GetValue(TipAlignmentProperty); }
            set { SetValue(TipAlignmentProperty, value); }
        }
        
        private static void OnTipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // quick and dirty fix to set the tooltip, because for some reason
            // `Content="{Binding Text, ElementName=Root}"` seems to not work, who knows why ¯\_(ツ)_/¯
            if (d is StateBox formFieldLabel)
            {
                var newTip = (string)e.NewValue;
                var tipElement = formFieldLabel.StateBoxToolTip;
                tipElement.Content = newTip;
                tipElement.Visibility = string.IsNullOrEmpty(newTip) ? Visibility.Hidden : Visibility.Visible;
            }
             
        }
        
        private static void OnTipAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // quick and dirty fix to set the tooltip, because for some reason
            // `Content="{Binding Text, ElementName=Root}"` seems to not work, who knows why ¯\_(ツ)_/¯
            if (d is StateBox formFieldLabel)
                formFieldLabel.StateBoxToolTip.Alignment = (ToolTipMessage.ToolTipAlignment)e.NewValue;
        }

        
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsDark
        {
            get { return (bool)GetValue(IsDarkProperty); }
            set { SetValue(IsDarkProperty, value); }
        }

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

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public StateBox()
        {
            InitializeComponent();
            StateBoxToolTip.Visibility = string.IsNullOrEmpty(TipText) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}