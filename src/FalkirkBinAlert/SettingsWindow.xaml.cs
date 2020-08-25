using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FalkirkBinAlert
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        private WebClient client = null;
        private ObservableCollection<UprnAddress> addresses = new ObservableCollection<UprnAddress>();
        private string originalUprn;

        public SettingsWindow()
        {
            InitializeComponent();
            AddressSelect.ItemsSource = addresses;
            var uprn = Properties.Settings.Default.Uprn;
            if (!string.IsNullOrEmpty(uprn))
                Uprn.Text = uprn;
        }

        private WebClient GetWebClient()
        {
            if (client == null)
            {
                client = new WebClient();
                client.DownloadStringCompleted += Client_DownloadStringCompleted;
            }
            return client;
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var options = GetAddressOptions(e.Result);
            if (options != null && options.Length > 0)
            {
                foreach (var option in options)
                {
                    addresses.Add(new UprnAddress
                    {
                        Uprn = option.GetAttribute("value").Trim(),
                        Address = option.TextContent,
                    });
                }
                AddressSelect.SelectedIndex = 0;
            }
            else
            {
                PostcodeError.Text = "Postcode not found.";
            }
        }

        private static IHtmlCollection<IElement> GetAddressOptions(string response)
        {
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(response);
            var options = doc.QuerySelectorAll("#T_FC_bodyContent_CPBody_ddProperties > option");
            return options;
        }

        private void PostcodeFind_Click(object sender, RoutedEventArgs e)
        {
            PostcodeError.Text = "";

            var postcode = PostCode.Text.Trim();
            if (postcode.Length > 0)
            {
                addresses.Clear();
                var client = GetWebClient();
                var url = "https://www.falkirk.gov.uk/services/bins-rubbish-recycling/household-waste/bin-collection-dates.aspx?postcode=";
                url += HttpUtility.UrlEncode(PostCode.Text);
                client.DownloadStringAsync(new Uri(url));
            }
        }

        private void MetroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }

        private void AddressSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (UprnAddress)AddressSelect.SelectedItem;
            if (item != null)
                Uprn.Text = item.Uprn;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var uprn = Uprn.Text.Trim();
            if (Regex.IsMatch(uprn, @"^\d+$"))
            {
                Properties.Settings.Default.Uprn = uprn;
                Properties.Settings.Default.Save();
                DialogResult = true;
            }
        }
    }
}
