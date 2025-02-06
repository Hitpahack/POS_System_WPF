using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class ZeroToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && System.Convert.ToInt32(value) == 0)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
