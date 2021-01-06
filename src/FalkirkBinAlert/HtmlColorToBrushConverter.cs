using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FalkirkBinAlert
{
    internal class HtmlColorToBrushConverter : IValueConverter
    {
        private readonly Dictionary<string, SolidColorBrush> brushes = new Dictionary<string, SolidColorBrush>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorCode = (string)value;
            if (!brushes.TryGetValue(colorCode, out SolidColorBrush brush))
            {
                brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
                brushes.Add(colorCode, brush);
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
