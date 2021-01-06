using System;
using System.Globalization;
using System.Windows.Data;

namespace FalkirkBinAlert
{
    internal class DaysToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int days = (int)value;

            string when;
            if (days == 0)
                when = "Today";
            else if (days == 1)
                when = "Tomorrow";
            else
                when = $"{days} days";

            return when;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
