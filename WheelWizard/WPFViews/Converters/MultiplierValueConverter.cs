using System;
using System.Globalization;
using System.Windows.Data;

namespace WheelWizard.WPFViews.Converters
{
    public class MultiplierValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double doubleValue) return value;

            var multiplier = 0.5; 
            if (parameter != null && double.TryParse(parameter.ToString(), NumberStyles.Any, 
                                                     CultureInfo.InvariantCulture, out var paramValue))
                multiplier = paramValue;
            
            return doubleValue * multiplier;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double doubleValue) return value;

            var multiplier = 0.5; 
           if (parameter != null && double.TryParse(parameter.ToString(), NumberStyles.Any,
                                                    CultureInfo.InvariantCulture, out var paramValue))
                           multiplier = paramValue;

            Console.WriteLine(multiplier);
            return doubleValue / multiplier;
        }
    }
}
