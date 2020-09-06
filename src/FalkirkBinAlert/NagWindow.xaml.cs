using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace FalkirkBinAlert
{
    /// <summary>
    /// Interaction logic for AlertWindow.xaml
    /// </summary>
    public partial class NagWindow : MetroWindow
    {
        //private DispatcherTimer clockTimer = new DispatcherTimer();
        //private int clockHours = 0;
        //private int clockMinutes = 0;

        public NagWindow(List<BinStatus> pendingBins)
        {
            InitializeComponent();
            PendingBins.ItemsSource = pendingBins;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Width - ActualWidth - 10;
            Top = workArea.Height - ActualHeight - 10;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
