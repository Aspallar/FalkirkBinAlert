using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace FalkirkBinAlert
{
    public class BinStatus
    {
        public BinStatus(string title, string colorCode, DateTime date)
        {
            Title = title;
            Color = colorCode;
            Date = date;
            DaysToCollection = DaysFronNow(Date);
        }

        public string Title { get; }

        public string Color { get; }

        public DateTime Date { get; }

        public int DaysToCollection { get; }

        public int Order => Title == BinTitles.BlackBox || Title == BinTitles.Food ? 2 : 1;

        public string InfoUrl
        {
            get
            {
                var url = "https://www.falkirk.gov.uk/services/bins-rubbish-recycling/household-waste/what-goes-in-my-bins/";
                switch (Title)
                {
                    case BinTitles.Green:
                        url += "green-bin.aspx";
                        break;
                    case BinTitles.Blue:
                        url += "blue-bin.aspx";
                        break;
                    case BinTitles.Burgundy:
                        url += "burgundy-bin.aspx";
                        break;
                    case BinTitles.Brown:
                        url += "brown-bin.aspx";
                        break;
                    case BinTitles.Food:
                        url += "food-waste.aspx";
                        break;
                    case BinTitles.BlackBox:
                        url += "black-box-textile-bag.aspx";
                        break;
                }
                return url;
            }
        }

        private static int DaysFronNow(DateTime date)
            => (int)(date - DateTime.Now.Date).TotalDays;
    }
}