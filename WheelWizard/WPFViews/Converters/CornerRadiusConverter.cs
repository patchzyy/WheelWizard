﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WheelWizard.WPFViews.Converters
{
    public class CornerRadiusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is CornerRadius && values[1] is Thickness)
            {
                var cornerRadius = (CornerRadius)values[0];
                var borderThickness = (Thickness)values[1];

                return new CornerRadius(
                    Math.Max(0, cornerRadius.TopLeft - borderThickness.Left),
                    Math.Max(0, cornerRadius.TopRight - borderThickness.Top),
                    Math.Max(0, cornerRadius.BottomRight - borderThickness.Right),
                    Math.Max(0, cornerRadius.BottomLeft - borderThickness.Bottom)
                );
            }

            return new CornerRadius(0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
