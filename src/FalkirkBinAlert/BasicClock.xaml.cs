﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;

namespace FalkirkBinAlert
{
    /// <summary>
    /// Interaction logic for BasicClock.xaml
    /// </summary>
    public partial class BasicClock : UserControl
    {
        private DispatcherTimer clockTimer = new DispatcherTimer();
        private int clockHours = 0;
        private int clockMinutes = 0;

        public BasicClock()
        {
            InitializeComponent();
            SetClockTime();
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += (s, e) => SetClockTime();
            clockTimer.Start();
        }

        private void SetClockTime()
        {
            var now = DateTime.Now.TimeOfDay;
            if (now.Hours > clockHours || now.Minutes > clockMinutes)
            {
                clockHours = now.Hours;
                clockMinutes = now.Minutes;
                Clock.Text = now.ToString(@"hh\:mm");
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            clockTimer.Stop();
            clockTimer = null;
        }
    }
}
