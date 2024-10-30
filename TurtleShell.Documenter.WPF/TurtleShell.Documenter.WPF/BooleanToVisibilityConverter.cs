using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TurtleShell.Documenter.WPF
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                bool invert = parameter != null && bool.Parse(parameter.ToString());
                if (invert)
                {
                    return boolean ? Visibility.Collapsed : Visibility.Visible;
                }
                return boolean ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool invert = parameter != null && bool.Parse(parameter.ToString());
                if (invert)
                {
                    return visibility != Visibility.Visible;
                }
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
