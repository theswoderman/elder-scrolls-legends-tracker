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
using ESLTracker.Utils;
using ESLTracker.ViewModels;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        new public MainWindowViewModel DataContext
        {
            get
            {
                return (MainWindowViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public static bool UpdateOverlay { get; internal set; }

        public MainWindow()
        {
            InitializeComponent();
            UpdateOverlayAsync(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.deckList.DataContext = DataModel.Tracker.Instance.Decks;
        }

        private static async void UpdateOverlayAsync(Window mainWindow)
        {
            OverlayToolbar ot = new OverlayToolbar();
            ot.Show();
            UpdateOverlay = true;
            while (UpdateOverlay && ! ot.IsDisposed())
            {
                ot.Visibility = WindowsUtils.IsGameActive() || ot.IsActive || mainWindow.IsActive ? Visibility.Visible : Visibility.Hidden;
               // ot.Topmost = WindowsUtils.IsGameActive();
                await Task.Delay(1000);
            }
            ot.Close();
            UpdateOverlay = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
            e.Cancel = true;
        }

        internal void RestoreOverlay()
        {
            if (!UpdateOverlay)
            {
                UpdateOverlayAsync(this);
            }
        }
    }
}