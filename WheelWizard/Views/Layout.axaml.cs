using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using WheelWizard.Services;
using WheelWizard.Views.Components;
using WheelWizard.Views.Pages;
using SettingsPage = WheelWizard.Views.Pages.Settings.SettingsPage;

namespace WheelWizard.Views;

public partial class Layout : Window
{
    public static Layout Instance { get; private set; }
    
    private UserControl _currentPage;
    
    public Layout()
    {
        Instance = this;
        InitializeComponent();
        NavigateToPage(new SettingsPage());
    }
    public void NavigateToPage(UserControl page)
    {
        ContentArea.Content = page;

        // Update the IsChecked state of the SidebarRadioButtons
        foreach (var child in SidePanelButtons.Children)
        {
            if (child is not SidebarRadioButton button) continue;

            var buttonPageType = button.PageType; 
            button.IsChecked = buttonPageType == page.GetType();
        }
    }

    private void TopBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e) => Close();
    private void MinimizeButton_Click(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void Discord_Click(object sender, EventArgs e) => ViewUtils.OpenLink(Endpoints.WhWzDiscordUrl);
    private void Github_Click(object sender, EventArgs e) =>  ViewUtils.OpenLink(Endpoints.WhWzGithubUrl);
    private void Support_Click(object sender, EventArgs e) => ViewUtils.OpenLink(Endpoints.SupportLink);
}

