using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class ExpiryDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _visibility = Visibility.Collapsed;

            if (value != null && value is DateTime _date)
            {
                if (_date <= DateTime.Now)
                    _visibility = Visibility.Visible;
            }
            return _visibility;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
