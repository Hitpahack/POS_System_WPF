using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class ArrayToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var _visibility = Visibility.Collapsed;


            var _collection = value as IEnumerable<object>;

            if (_collection != null && _collection.Any())
            {
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
