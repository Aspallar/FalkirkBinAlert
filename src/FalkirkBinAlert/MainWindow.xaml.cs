using System;
using System.Collections.Generic;
using System.Linq;
using MahApps.Metro.Controls;
using System.Windows;
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
        private FalkirkWebClient client;
        private DispatcherTimer refreshTimer = new DispatcherTimer();
        private readonly ObservableCollection<BinStatus> binStatus = new ObservableCollection<BinStatus>();

        private DispatcherTimer nagTimer;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private bool running = true;

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
                UpdateNagTimer();
            }
            else
            {
                BinStatusList.Visibility = Visibility.Hidden;
                NetworkError.Visibility = Visibility.Visible;
                StartRefreshTimer(true);
            }
        }

        private void UpdateNagTimer()
        {
            var settings = Properties.Settings.Default;
            if (binStatus.Any(x => x.WhenDays == 1)) {
                var nagStart = Properties.Settings.Default.NagStartTime;
                var now = DateTime.Now.TimeOfDay;
                var interval = now < nagStart ? nagStart - now : TimeSpan.FromMinutes(2);
                if (nagTimer == null)
                {
                    nagTimer = new DispatcherTimer();
                    nagTimer.Tick += NagTimer_Tick;
                }
                nagTimer.Interval = interval;
                nagTimer.Start();
            }
            else if (nagTimer != null)
            {
                nagTimer.Stop();
                nagTimer = null;
            }
        }

        private void NagTimer_Tick(object sender, EventArgs e)
        {
            nagTimer.Stop();
            var dlg = new NagWindow() { Owner = this };
            if ((bool)dlg.ShowDialog())
            {
                nagTimer = null;
            }
            else
            {
                nagTimer.Interval = Properties.Settings.Default.NagInterval;
                nagTimer.Start();
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
            InitializeNotifyIcon();
            FetchBinData();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Uprn))
                Visibility = Visibility.Hidden;
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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (running)
            {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
                var settings = Properties.Settings.Default;
                if (!settings.RunningWarningShown)
                {
                    notifyIcon.ShowBalloonTip(6000,
                        "Falkirk Bins is still running",
                        "Right click it's icon in the System Tray and choose Exit to terminate. This message will not be shown again.",
                        System.Windows.Forms.ToolTipIcon.Info);
                    settings.RunningWarningShown = true;
                    settings.Save();
                }
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = Icon.ToSystemDrawingIcon(),
                Visible = true,
                ContextMenu = new System.Windows.Forms.ContextMenu(),
                Text = "Falkirk Bins",
            };

            notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Show", NotifyIcon_Click));
            notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Exit", NotifyIcon_Exit_Click));
            notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.BalloonTipClicked += NotifyIcon_Click;
        }

        private void NotifyIcon_Exit_Click(object sender, EventArgs e)
        {
            running = false;
            Close();
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
            Activate();
        }
    }
}
