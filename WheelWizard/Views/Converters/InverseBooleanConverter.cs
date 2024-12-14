using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WheelWizard.Views.Converters;

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolean)
            return !boolean;
        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolean)
            return !boolean;
        return AvaloniaProperty.UnsetValue;
    }
}
