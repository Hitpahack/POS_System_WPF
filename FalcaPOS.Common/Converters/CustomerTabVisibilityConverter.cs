using FalcaPOS.Common.Constants;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class CustomerTabVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _visibility = false;

            if (AppConstants.USER_ROLES != null)
            {
                _visibility = (AppConstants.USER_ROLES.Contains("admin") || AppConstants.USER_ROLES.Contains("finance") || AppConstants.USER_ROLES.Contains("falcadirector")
                    || AppConstants.USER_ROLES.Contains("storeperson")|| AppConstants.USER_ROLES.Contains("controlmanager"))
                    ? true : false;
            }

            return _visibility;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
