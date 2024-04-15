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

namespace DemoEx
{
    /// <summary>
    /// Логика взаимодействия для download.xaml
    /// </summary>
    public partial class download : Page
    {
        private DispatcherTimer timer;
        public download()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval=TimeSpan.FromSeconds(5);
            timer.Tick += Timer_tick;
            timer.Start();

        }

        private void Timer_tick(object sender, EventArgs e)
        {
            timer.Stop();
            NavigationService.Navigate(new auto());
        }
    }
}
