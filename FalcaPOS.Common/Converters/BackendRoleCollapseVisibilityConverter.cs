using FalcaPOS.Common.Constants;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class BackendRoleCollapseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _visibility = Visibility.Collapsed;

            if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Any())
            {
                _visibility = AppConstants.USER_ROLES.Any(x => x == AppConstants.ROLE_BACKEND) ? Visibility.Collapsed : Visibility.Visible;
            }

            return _visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
