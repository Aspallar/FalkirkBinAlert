using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
        private bool runOnStartup;

        private readonly List<string> nagIntervalText = new List<string>
        {
            "15 minutes",
            "30 minutes",
            "1 hour",
            "1.5 hours",
            "2 hours",
        };
        private readonly List<int> nagIntervals = new List<int> { 15, 30, 60, 90, 120 };

        public SettingsWindow()
        {
            InitializeComponent();

            var settings = Properties.Settings.Default;

            AddressSelect.ItemsSource = addresses;
            NagEvery.ItemsSource = nagIntervalText;
            NagStart.Culture = CultureInfo.CreateSpecificCulture("en-gb");
            NagStart.SelectedDateTime = DateTime.Now.Date + settings.NagStartTime;
            PlayAudio.IsChecked = settings.PlayNagAudio;

            var mins = (int)settings.NagInterval.TotalMinutes;
            NagEvery.SelectedIndex = nagIntervals.IndexOf(mins);

            if (!string.IsNullOrEmpty(settings.Uprn))
                Uprn.Text = settings.Uprn;

            RunOnStartup.IsChecked = runOnStartup = AppRegistry.RunOnStartup;
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
            FindPostcode();
        }

        private void FindPostcode()
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
            UpdateApplicationSettings();
            UpdateRunOnStartup();
            DialogResult = true;
        }

        private void UpdateRunOnStartup()
        {
            if (RunOnStartup.IsChecked.Value != runOnStartup)
                AppRegistry.RunOnStartup = RunOnStartup.IsChecked.Value;
        }

        private void UpdateApplicationSettings()
        {
            var settings = Properties.Settings.Default;
            var uprn = Uprn.Text.Trim();
            if (Regex.IsMatch(uprn, @"^\d+$"))
                settings.Uprn = uprn;
            settings.Theme = ThemePicker.ThemeName;
            if (NagStart.SelectedDateTime.HasValue)
                settings.NagStartTime = NagStart.SelectedDateTime.Value.TimeOfDay;
            settings.NagInterval = TimeSpan.FromMinutes(nagIntervals[NagEvery.SelectedIndex]);
            settings.PlayNagAudio = PlayAudio.IsChecked.Value;
            settings.Save();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var settings = Properties.Settings.Default;
            if (settings.Theme != ThemePicker.ThemeName)
                ThemeManager.Current.ChangeTheme(Application.Current, settings.Theme);
        }

        private void PostCode_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                FindPostcode();
            }
        }
    }
}
