using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Baku.MagicMirror.Kinect
{
    class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return DependencyProperty.UnsetValue;
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return DependencyProperty.UnsetValue;
            return !((bool)value);
        }
    }
}
