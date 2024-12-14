using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WheelWizard.Views.Converters;

public class BoolOrConverter : IMultiValueConverter
{

    public object? Convert(IList<object?>? values, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (values is null || values.Count == 0)
            return false;

        return values.Any(v => v is (bool and true));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) 
        => throw new NotSupportedException();
}
