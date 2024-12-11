using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using WheelWizard.Services;
using WheelWizard.Views.Components;
using WheelWizard.Views.Pages;

namespace WheelWizard.Views;

public partial class Layout : Window
{
    private UserControl _currentPage;
    
    
    public Layout()
    {
        InitializeComponent();
        NavigateToPage(new SettingsPage());
    }
    public void NavigateToPage(UserControl page)
    {
        ContentArea.Content = page;

        // Update the IsChecked state of the SidebarRadioButtons
        foreach (var child in SidePanelButtons.Children)
        {
            if (child is SidebarRadioButton button)
            {
                // Assuming you have a way to associate a page type with each button
                // For example, you could use a custom property or the Tag property
                var buttonPageType = button.Tag as Type; 
                button.IsChecked = buttonPageType == page.GetType();
            }
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

