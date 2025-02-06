using System;
using System.Globalization;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class MultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null)
            {
                Tuple<object, object> tuple = new Tuple<object, object>(values[0], values[1]);

                return (object)tuple;
            }
            return null;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
