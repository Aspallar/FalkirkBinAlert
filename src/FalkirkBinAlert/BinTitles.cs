using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalkirkBinAlert
{
    public static class BinTitles
    {
        public const string Green = "Green bin";
        public const string Blue = "Blue bin";
        public const string Brown = "Brown bin";
        public const string Burgundy = "Burgundy bin";
        public const string Food = "Food caddy";
        public const string BlackBox = "Black box";

        public static ReadOnlyCollection<string> AllBinTitles = new ReadOnlyCollection<string>
        (
            new List<string> { Green, Blue, Brown, Burgundy, Food, BlackBox }
        );
    }
}
