using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FalcaPOS.Common.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            if (value == null)
            {
                return new SolidColorBrush(Colors.Black);
            }
            else if (((DateTime)value).Date <= DateTime.Now.Date)
            {
                return new SolidColorBrush(Colors.Red);
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
