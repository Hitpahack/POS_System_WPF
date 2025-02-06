using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FalcaPOS.Common.Converters
{
    public class DepartmentColorCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            if (value == null)
            {
                return new SolidColorBrush(Colors.Black);
            }
            else if (value.ToString().ToLower() == "plant protection chemicals")
            {
                return "#9584ea";
            }
            else if (value.ToString().ToLower() == "fertilizer")
            {
                return "#c6394a";
            }
            else if (value.ToString().ToLower() == "seeds field crops")
            {
                return "#326da8";
            }
            else if (value.ToString().ToLower() == "seeds horticulture crops")
            {
                return "#5a308b";
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }




    }
}
