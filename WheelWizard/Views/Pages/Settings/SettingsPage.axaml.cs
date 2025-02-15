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
        
        WhWzVersionText.Text = "WhWz: v" + AutoUpdaterWindows.CurrentVersion;
        RrVersionText.Text = "RR: " + RetroRewindInstaller.CurrentRRVersion();

        var part1 = "Release";
        var part2 = "Unknown OS";
#if DEBUG
        part1 = "Dev";
        DevButton.IsVisible = true;
#endif
       
#if WINDOWS
       part2 = "Windows";
#elif LINUX
        part2 = "Linux";
#elif MACOS
        part2 = "Macos";
#endif
        
        ReleaseText.Text = $"{part1} - {part2}";
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
    
    private void DevButton_OnClick(object? sender, RoutedEventArgs e) => new DevToolWindow().Show();
}

