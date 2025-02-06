using FalcaPOS.Common.Constants;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class GridWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (AppConstants.USER_ROLES.Length > 0 &&
                AppConstants.USER_ROLES.Contains(AppConstants.ROLE_FINANCE.ToString()))
                return 100;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
