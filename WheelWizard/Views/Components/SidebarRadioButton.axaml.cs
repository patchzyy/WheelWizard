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
            else
            {
                // Very detailed Console.WriteLine for debugging:
                Console.WriteLine("Navigation failed from SidebarRadioButton.OnClick:");
        
                if (layoutWindow == null)
                {
                    Console.WriteLine("  - Could not find the parent Layout window.");
                    Console.WriteLine("  - Ensure that the SidebarRadioButton is a descendant of the Layout window in the visual tree.");
                }
        
                if (PageType == null)
                {
                    Console.WriteLine("  - PageType property is null.");
                    Console.WriteLine("  - Verify that the PageType property is correctly set in the XAML or code-behind where the SidebarRadioButton is defined.");
                    Console.WriteLine($"  - Current SidebarRadioButton: Text = '{this.Text}', IconData = '{this.IconData}'");
                    Console.WriteLine("  - Check if the Tag property (or whichever property you are using to store the page type) is set to the correct Type using x:Type in XAML.");
                }
        
                if (layoutWindow != null && PageType != null) {
                    Console.WriteLine("  - layoutWindow and PageType are not null but page creation failed");
                }
        
                Console.WriteLine("  - Additional debugging tips:");
                Console.WriteLine("    - Set a breakpoint in this method and inspect the values of layoutWindow and PageType.");
                Console.WriteLine("    - Check the Output window in your IDE for any exceptions or error messages related to navigation.");
                Console.WriteLine("    - If you are using a custom property to store the page type, ensure that it is a public property of type Type.");
            }
        }
    }
}
