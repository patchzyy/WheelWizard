using System.Windows;
using System.Windows.Controls;

/*
EXAMPLES:

# all parameters
 <components:Button Variant="Primary"
                    IsEnabled="True"
                    IconPack="Material"
                    IconKind="{x:Static icon:PackIconMaterialKind.Home}"
                    IconSize="20"
                    Text="Home"
                    FontSize="16"
                    Click="Button_OnClick"/>

# but you dont need all
 <components:Button IconSize="0"
                    Text="Home"
                    Click="Button_OnClick"/>
                    
# Note1:  you either need to find the icon, or set IconSize to 0 to hide it
# Note2:  variant can be: Primary, Secondary, Default, Danger   (if none, it will go to Default)
 */

namespace CT_MKWII_WPF.Views.Components
{
    public partial class Button : UserControl
    {
        // Primary, Secondary, Default, Danger
        public static readonly DependencyProperty VariantProperty =
            DependencyProperty.Register(nameof(Variant), typeof(string), typeof(Button), 
                new PropertyMetadata("Default"));
        
        public string Variant
        {
            get { return (string)GetValue(VariantProperty); }
            set { SetValue(VariantProperty, value); }
        }
        
        public static readonly new DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(Button), 
                new PropertyMetadata(true));
     
        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }
        
        public static readonly DependencyProperty IconPackProperty =
            DependencyProperty.Register(nameof(IconPack), typeof(string), typeof(Button),
                new PropertyMetadata(null));

        public string IconPack
        {
            get { return (string)GetValue(IconPackProperty); }
            set { SetValue(IconPackProperty, value); }
        }

        
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(object), typeof(Button),
                new PropertyMetadata(null));
            
        public object IconKind
        {
            get { return GetValue(IconKindProperty); }
            set { SetValue(IconKindProperty, value); }
        }
        
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(Button), 
                new PropertyMetadata(20.0));
        
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Button), 
                new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        public static readonly new DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(Button), 
                new PropertyMetadata(16.0));
   
        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        
        public Button()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Raise a custom click event or execute a command here
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(newEventArgs);
        }

        // Custom click event
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Button));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
    }
}
