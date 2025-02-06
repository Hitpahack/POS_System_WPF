using System;
using System.Globalization;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class MultiValueHeaderFormaterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {


            //check for from date and to date
            //return "new aherdd";

            //string _headerName=""

            //for


            return parameter;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
