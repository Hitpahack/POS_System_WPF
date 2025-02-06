using FalcaPOS.Common.Constants;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class DeleteInvoiceFileVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Any(CheckRole) ? Visibility.Visible : (object)Visibility.Collapsed;
        }

        private bool CheckRole(string role) => role == AppConstants.ROLE_FINANCE;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
