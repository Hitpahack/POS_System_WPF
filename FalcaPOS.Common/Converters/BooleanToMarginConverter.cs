using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FalcaPOS.Common.Converters
{
    /// <summary>
    /// This class holds the conversion of a boolean value into margin.
    /// </summary>
    public class BooleanToMarginConverter : IValueConverter
    {
        /// <summary>
        /// This method sets the margin based on the boolean value passed from source.
        /// </summary>
        /// <param name="value">value passed from source</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">parameter</param>
        /// <param name="culture">culture info</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Sets the default margin to 0,0,0,0.
            var _margin = new Thickness(0, 0, 0, 0);

            // If the value passed is not null and boolean type
            if (value != null && value is bool)
            {
                // If the value passed is true, sets the margin to 0,20,0,0.
                if ((bool)value)
                {
                    _margin = new Thickness(0, 20, 0, 0);
                }
            }
            return _margin;
        }

        /// <summary>
        /// Converts the value back to the type.
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="targetType">target type<param>
        /// <param name="parameter">parameter<param>
        /// <param name="culture">culture</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
