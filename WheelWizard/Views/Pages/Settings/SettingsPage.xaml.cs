using CT_MKWII_WPF.Services;
using CT_MKWII_WPF.Services.Installation;
using CT_MKWII_WPF.Services.Settings;
using CT_MKWII_WPF.Services.WiiManagement;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static CT_MKWII_WPF.Views.ViewUtils;
using Button = CT_MKWII_WPF.Views.Components.Button;

namespace CT_MKWII_WPF.Views.Pages.Settings;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        
        WhWzVersionText.Text = "WhWz: v" + AutoUpdater.CurrentVersion;
        RrVersionText.Text = "RR: " + RetroRewindInstaller.CurrentRRVersion();
        SettingsContent.Content = new WhWzSettings();
        
    }

    private void TopBarRadio_OnClick(object sender, RoutedEventArgs e)
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
}
