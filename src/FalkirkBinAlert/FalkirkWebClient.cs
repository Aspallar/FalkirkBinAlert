using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace FalkirkBinAlert
{
    public class FalkirkWebClient : WebClient
    {
        private const string dateFormat = "yyyy-MM-dd";

        public FalkirkWebClient() : base()
        {
            Encoding = Encoding.UTF8;
            BaseAddress = "https://www.falkirk.gov.uk";
        }

        public void FetchCalendarDataAsync(string uprn, DateTime start, DateTime end)
        {

            var startParam = start.ToString(dateFormat);
            var endParam = end.ToString(dateFormat);
            var uri = new Uri($"bin-calendar?uprn={uprn}&start={startParam}&end={endParam}", UriKind.Relative);
            DownloadStringAsync(uri);
        }

        public void FetchAddressPageAsync(string postCode)
        {
            var url = "services/bins-rubbish-recycling/household-waste/bin-collection-dates.aspx?postcode="
                + HttpUtility.UrlEncode(postCode);
            DownloadStringAsync(new Uri(url, UriKind.Relative));
        }

        private string UserAgent
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var versionString = $"{version.Major}.{version.Minor}.{version.Build}";
                return $"FalkirkBinAlert/{versionString} (https://github.com/Aspallar/FalkirkBinAlert)";
            }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest) base.GetWebRequest(address);
            request.AllowAutoRedirect = false;
            request.UserAgent = UserAgent;
            return request;
        }
    }
}
