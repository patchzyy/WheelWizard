using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;

namespace WheelWizard.Views.Components
{
    public partial class SidebarRadioButton : UserControl
    {
        public static readonly StyledProperty<Geometry> IconDataProperty = 
            AvaloniaProperty.Register<SidebarRadioButton, Geometry>(nameof(IconData), new PathGeometry());
        public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(Text), "Sidebar");
        public static readonly StyledProperty<string> BoxTextProperty = AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(BoxText));
        public static readonly StyledProperty<IImage?> BoxIconDataProperty = AvaloniaProperty.Register<SidebarRadioButton, IImage?>(nameof(BoxIconData));
        public static readonly StyledProperty<string> BoxTipProperty = AvaloniaProperty.Register<SidebarRadioButton, string>(nameof(BoxTip));
        public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<SidebarRadioButton, bool>(nameof(IsChecked), false);
        public static readonly StyledProperty<bool> IsIconLeftProperty = AvaloniaProperty.Register<SidebarRadioButton, bool>(nameof(IsIconLeft), true);

        public Geometry IconData
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

        public Type PageType { get; set; }
        public SidebarRadioButton()
        {
            InitializeComponent();
        }

        private void OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IsChecked = true;
        
            // Find the parent Layout window and navigate
            var layoutWindow = this.FindAncestorOfType<Layout>();
            if (layoutWindow != null && PageType != null)
            {
                // Create an instance of the page type and navigate
                var page = Activator.CreateInstance(PageType) as UserControl;
                layoutWindow.NavigateToPage(page);
            }
        }
    }
}
