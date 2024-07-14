using System.Windows;
using System.Windows.Controls;

/*
 This component can be used in any page that is not yet developed. or in a page that should list content, but there is none.
 
 EXAMPLES:
       <components:EmptyPageInfo 
                Title="Awww, empty page!"
                Body="This page is still in development. You can expect this page to be filled with content in the future." />
 */

namespace CT_MKWII_WPF.Views.Components
{
    public partial class EmptyPageInfo : UserControl
    {
        public EmptyPageInfo() => InitializeComponent();

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(EmptyPageInfo), 
            new PropertyMetadata("This is empty!")
            );

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty BodyProperty = DependencyProperty.Register(
            nameof(Body), typeof(string), typeof(EmptyPageInfo), 
            new PropertyMetadata("It seems like there is no content to  display here.")
            );

        public string Body
        {
            get => (string)GetValue(BodyProperty);
            set => SetValue(BodyProperty, value);
        }
    }
}
