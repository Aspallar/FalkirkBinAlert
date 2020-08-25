using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using MahApps.Metro.Controls;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FalkirkBinAlert
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        private FalkirkWebClient client = null;
        private ObservableCollection<UprnAddress> addresses = new ObservableCollection<UprnAddress>();

        public SettingsWindow()
        {
            InitializeComponent();
            AddressSelect.ItemsSource = addresses;
            var uprn = Properties.Settings.Default.Uprn;
            if (!string.IsNullOrEmpty(uprn))
                Uprn.Text = uprn;
        }

        private FalkirkWebClient GetWebClient()
        {
            if (client == null)
            {
                client = new FalkirkWebClient();
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
                client.FetchAddressPageAsync(PostCode.Text);
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
