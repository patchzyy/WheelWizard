using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using WheelWizard.Services.Installation;
using WheelWizard.Views.Popups;
using WheelWizard.Views.Popups.Generic;

namespace WheelWizard.Views.Pages.Settings;

public partial class SettingsPage : UserControl
{
    public SettingsPage() : this(new WhWzSettings()) { }
    public SettingsPage(UserControl initialSettingsPage)
    {
        InitializeComponent();
        
        WhWzVersionText.Text = "WhWz: v" + AutoUpdater.CurrentVersion;
        RrVersionText.Text = "RR: " + RetroRewindInstaller.CurrentRRVersion();
        
    #if RELEASE_BUILD
        ReleaseText.IsVisible = false;
        DevButton.IsVisible = false;
    #endif
        
        SettingsContent.Content = initialSettingsPage;
    }

    private void TopBarRadio_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton) 
            return;
        
        // As long as the Ks... files are next to this file, it works. 
        var namespaceName = GetType().Namespace;
        var typeName = $"{namespaceName}.{radioButton.Tag}";
        var type = Type.GetType(typeName);
        if (type == null || !typeof(UserControl).IsAssignableFrom(type)) 
            return;

        if (Activator.CreateInstance(type) is not UserControl instance) 
            return;
        
        SettingsContent.Content = instance;
    }
    
    private void DevButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var popup = new MessageBoxWindow()
            .SetMainText("Hey plzz respond")
            .SetExtraText("Something else");
        popup.Show();
    }
}

