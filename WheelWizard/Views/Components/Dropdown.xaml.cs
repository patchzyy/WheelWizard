using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CT_MKWII_WPF.Views.Components;

public partial class Dropdown : ComboBox
{
    public Dropdown()
    {
        InitializeComponent();
        FontSize = 16.0;
        MaxDropDownHeight = 225.0;
    }
    
   private void DropDown_SizeChanged(object sender, SizeChangedEventArgs e)
   {
       if (sender is not Border border)
           return;
       
       border.Clip = new RectangleGeometry
       {
           Rect = new Rect(0, 0, border.ActualWidth, border.ActualHeight),
           RadiusX = 6,
           RadiusY = 6
       };
   }
}

