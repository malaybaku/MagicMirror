using System;
using System.Globalization;

using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Baku.MagicMirror.LED
{
    class ColorModelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cm = value as ColorModel;
            if (cm == null) return DependencyProperty.UnsetValue;

            return new SolidColorBrush(Color.FromRgb(cm.R, cm.G, cm.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
