using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class IconLabel : UserControl
    {
        public IconLabel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Brush), typeof(IconLabel),
            new PropertyMetadata(Brushes.Black, OnColorChanged));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register(
            nameof(IconColor), typeof(Brush), typeof(IconLabel),
            new PropertyMetadata(Brushes.Black));

        public Brush IconColor
        {
            get => (Brush)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
            nameof(TextColor), typeof(Brush), typeof(IconLabel),
            new PropertyMetadata(Brushes.Black));

        public Brush TextColor
        {
            get => (Brush)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not IconLabel iconLabel) return;

            iconLabel.IconColor = (Brush)e.NewValue;
            iconLabel.TextColor = (Brush)e.NewValue;
        }
        
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
            nameof(IconSize), typeof(double), typeof(IconLabel),
            new PropertyMetadata(16.0));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(IconLabel),
            new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register(
            nameof(IconKind), typeof(object), typeof(IconLabel),
            new PropertyMetadata(null));

        public object IconKind
        {
            get => GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public static readonly DependencyProperty IconPackProperty = DependencyProperty.Register(
            nameof(IconPack), typeof(string), typeof(IconLabel),
            new PropertyMetadata(null));

        public string IconPack
        {
            get => (string)GetValue(IconPackProperty);
            set => SetValue(IconPackProperty, value);
        }
    }
}
