using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FalcaPOS.Common.Converters {
    public class IndentPaymentStatusConverter: IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {


            if (value == null) {
                return "";
            }
            else if ((bool)value) {
                return "Full";
            }
            else if (!(bool)value) {
                return "Partial";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
