using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using WheelWizard.Services;

namespace WheelWizard.Views;

public partial class Layout : Window
{
    public Layout()
    {
        InitializeComponent();
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

