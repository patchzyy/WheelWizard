using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace WheelWizard.Views.Components
{
    public partial class SidebarRadioButton : UserControl
    {
        public static readonly StyledProperty<IImage?> IconDataProperty = AvaloniaProperty.Register<SidebarRadioButton, IImage?>(nameof(IconData));
        public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(Text));
        public static readonly StyledProperty<string> BoxTextProperty = AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(BoxText));
        public static readonly StyledProperty<IImage?> BoxIconDataProperty = AvaloniaProperty.Register<SidebarRadioButton, IImage?>(nameof(BoxIconData));
        public static readonly StyledProperty<string> BoxTipProperty = AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(BoxTip));
        public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<SidebarRadioButton, bool>(nameof(IsChecked), false);
        public static readonly StyledProperty<bool> IsIconLeftProperty = AvaloniaProperty.Register<SidebarRadioButton, bool>(nameof(IsIconLeft), true);

        public IImage? IconData
        {
            get => GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string BoxText
        {
            get => GetValue(BoxTextProperty);
            set => SetValue(BoxTextProperty, value);
        }

        public IImage? BoxIconData
        {
            get => GetValue(BoxIconDataProperty);
            set => SetValue(BoxIconDataProperty, value);
        }

        public string BoxTip
        {
            get => GetValue(BoxTipProperty);
            set => SetValue(BoxTipProperty, value);
        }

        public bool IsChecked
        {
            get => GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        
        public bool IsIconLeft
        {
            get => GetValue(IsIconLeftProperty);
            set => SetValue(IsIconLeftProperty, value);
        }

        public SidebarRadioButton()
        {
            InitializeComponent();
        }

        private void OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IsChecked = true;
        }
    }
}
