using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace WheelWizard.Views.Pages;

public partial class HomePage : UserControl
{
    private double _wheelRotationAngle = 0;
    public HomePage()
    {
        InitializeComponent();
        StartWheelRotation();
    }

    private async void StartWheelRotation()
    {
        // while (true) // Infinite loop for continuous rotation
        // {
        //     _wheelRotationAngle = (_wheelRotationAngle + 0.1) % 360; // Increment the angle, reset at 360
        //     // WheelRotation.Angle = _wheelRotationAngle; // Update the rotation angle
        //     await Task.Delay(10); // A small delay to control the speed of rotation
        // }
    }
    

    private void DolphinButton_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void PlayButton_Click(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
