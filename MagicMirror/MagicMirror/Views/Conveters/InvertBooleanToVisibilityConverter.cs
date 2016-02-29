using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Baku.MagicMirror.Views
{
    class InvertBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is bool) || (targetType != typeof(Visibility)))
            {
                return Binding.DoNothing;
            }

            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;

    }
}
