using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace FalkirkBinAlert
{
    internal class BinStatus
    {
        private static readonly Dictionary<string, SolidColorBrush> colors = new Dictionary<string, SolidColorBrush>();

        public BinStatus(string title, string colorCode, DateTime date)
        {
            Title = title;
            Color = Brush(colorCode);
            Date = date;
            When = when(date);
            Day = date.ToString("ddd");
        }

        public string Title { get; }

        public SolidColorBrush Color { get; }

        public DateTime Date { get; }

        public string When { get; }

        public string Day { get; } 

        private static string when(DateTime date)
        {
            var now = DateTime.Now.Date;
            var days = (date - now).TotalDays;

            string when;
            if (days == 0)
                when = "Today";
            else if (days == 1)
                when = "Tommorow";
            else
                when = $"{days} days";

            return when;
        }

        private static SolidColorBrush Brush(string colorCode)
        {
            if (!colors.TryGetValue(colorCode, out SolidColorBrush brush))
            {
                brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
                colors.Add(colorCode, brush);
            }
            return brush;
        }

    }
}