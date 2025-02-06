using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Visibility _visibility = Visibility.Collapsed;

            if (value != null)
            {
                try
                {
                    bool _value = System.Convert.ToBoolean(value);
                    if (!_value)
                        _visibility = Visibility.Visible;
                    else
                        _visibility = Visibility.Collapsed;
                }
                catch (Exception)
                {
                }
            }
            return _visibility;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
