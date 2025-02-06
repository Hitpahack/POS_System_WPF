using FalcaPOS.Common.Constants;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class SuperBackendTabVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _visibility = false;

            if (AppConstants.USER_ROLES != null)
            {
                _visibility = AppConstants.USER_ROLES.Contains("purchasemanager") ? true : false;
            }

            return _visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
