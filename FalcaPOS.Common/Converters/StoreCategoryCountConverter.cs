using System;
using System.Globalization;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class StoreCategoryCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return System.Convert.ToInt32(value) > 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
