using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    public class StoreImagesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string resourceKey = value as string;
            if (!String.IsNullOrEmpty(resourceKey))
            {
                var resource = Application.Current.FindResource(resourceKey);
                if (resource != null)
                {
                    return resource;
                }
            }
            return Binding.DoNothing;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
