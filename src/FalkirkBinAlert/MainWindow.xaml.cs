using System;
using System.Collections.Generic;
using System.Linq;
using MahApps.Metro.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Threading;

namespace FalkirkBinAlert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private FalkirkWebClient client = null;
        private DispatcherTimer refreshTimer = new DispatcherTimer();
        private readonly ObservableCollection<BinStatus> binStatus = new ObservableCollection<BinStatus>();

        public MainWindow()
        {
            InitializeComponent();
            BinStatusList.ItemsSource = binStatus;
            refreshTimer.Tick += RefreshTimer_Tick;
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            FetchBinData();
        }

        private void FetchBinData()
        {
            var uprn = Properties.Settings.Default.Uprn;
            if (string.IsNullOrEmpty(uprn))
            {
                NoLocation.Visibility = Visibility.Visible;
            }
            else
            {
                NoLocation.Visibility = Visibility.Hidden;
                refreshTimer.Stop();
                var start = DateTime.Now.Date;
                var end = start.AddDays(90);
                client = new FalkirkWebClient();
                client.DownloadStringCompleted += Client_DownloadStringCompleted;
                client.FetchCalendarDataAsync(uprn, start, end);
            }
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            client.Dispose();
            client = null;

            if (!e.Cancelled && e.Error == null)
            {
                UpdateBins(e.Result);
                BinStatusList.Visibility = Visibility.Visible;
                NetworkError.Visibility = Visibility.Hidden;
                StartRefreshTimer(false);
            }
            else
            {
                BinStatusList.Visibility = Visibility.Hidden;
                NetworkError.Visibility = Visibility.Visible;
                StartRefreshTimer(true);
            }
        }

        private void UpdateBins(string binJson)
        {
            var binNames = new List<string> { "Green bin", "Blue bin", "Burgundy bin", "Brown bin", "Food caddy", "Black box" };
            var binData = JsonConvert.DeserializeObject<List<BinEntry>>(binJson);

            binData = binData.OrderBy(x => x.Start).ToList();
            var status = new List<BinStatus>();
            foreach (var name in binNames)
            {
                var entry = binData.Where(x => x.Title == name).FirstOrDefault();
                if (entry != null)
                    status.Add(new BinStatus(entry.Title, entry.Color, entry.Start));
            }
            binStatus.Clear();
            foreach (var bin in status.OrderBy(x => x.Date))
                binStatus.Add(bin);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FetchBinData();
        }

        private void StartRefreshTimer(bool isRetry)
        {
            TimeSpan interval;
            if (isRetry)
            {
                interval = TimeSpan.FromMinutes(5);
            }
            else
            {
                var now = DateTime.Now;
                var when = now.Date.AddDays(1).AddSeconds(1);
                interval = when - now;
            }
            refreshTimer.Interval = interval;
            refreshTimer.Start();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SettingsWindow { Owner = this };
            if ((bool)dlg.ShowDialog())
            {
                FetchBinData();
            }
        }
    }
}
