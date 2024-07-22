using System;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages.KitchenSink;

public partial class KitchenSink : Page
{
    public KitchenSink()
    {
        InitializeComponent();
        KitchenSinkContent.Content = new KsGeneric();
    }
    
    private void TopBarRadio_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton) return;
        
        // As long as the Ks... files are next to this file, it works. 
        var namespaceName = GetType().Namespace;
        var typeName = $"{namespaceName}.{radioButton.Tag}";
        var type = Type.GetType(typeName);
        if (type == null || !typeof(UserControl).IsAssignableFrom(type)) return;

        if (Activator.CreateInstance(type) is not UserControl instance) return;
        
        KitchenSinkContent.Content = instance;
    }
}
