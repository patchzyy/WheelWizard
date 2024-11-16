using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

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

    private void CloseButton_Click(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void MinimizeButton_Click(object? sender, RoutedEventArgs e) => Close();
}
