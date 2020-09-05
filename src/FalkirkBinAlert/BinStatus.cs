﻿using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace FalkirkBinAlert
{
    public class BinStatus
    {
        private static readonly Dictionary<string, SolidColorBrush> colors = new Dictionary<string, SolidColorBrush>();

        public BinStatus(string title, string colorCode, DateTime date)
        {
            Title = title;
            Color = Brush(colorCode);
            Date = date;
            WhenDays = Days(date);
            When = when(WhenDays);
            Day = date.ToString("ddd");
        }

        public string Title { get; }

        public SolidColorBrush Color { get; }

        public DateTime Date { get; }

        public int WhenDays { get; }

        public string When { get; }

        public string Day { get; }


        public int Order => Title == "Black box" || Title == "Food caddy" ? 2 : 1;

        private static int Days(DateTime date)
            => (int)(date - DateTime.Now.Date).TotalDays;

        private static string when(int days)
        {
            string when;
            if (days == 0)
                when = "Today";
            else if (days == 1)
                when = "Tomorrow";
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

        public static int OrderValue(BinStatus bin)
            => bin.Title == "Black box" || bin.Title == "Food caddy" ? 2 : 1;

    }
}