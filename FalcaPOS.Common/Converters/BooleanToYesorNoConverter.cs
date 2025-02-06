using System;
using System.Globalization;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class BooleanToYesorNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool _val)
            {
                return _val ? "Yes" : "No";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
