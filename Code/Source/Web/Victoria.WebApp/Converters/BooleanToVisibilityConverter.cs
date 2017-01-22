using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;
using MS.Internal.Controls;


namespace Victoria.WebApp.Converters
{
    /// <summary>
    /// Convert between boolean and visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary> 
        /// Convert bool or Nullable<bool> to Visibility
        /// </summary> 
        /// <param name="value">bool or Nullable<bool>
        /// <param name="targetType">Visibility
        /// <param name="parameter">null
        /// <param name="culture">null 
        /// <returns>Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            return (bValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Convert Visibility to boolean 
        /// </summary>
        /// <param name="value"> 
        /// <param name="targetType"> 
        /// <param name="parameter">
        /// <param name="culture"> 
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Visible;
            }
            else
            {
                return false;
            }
        }
    }
}