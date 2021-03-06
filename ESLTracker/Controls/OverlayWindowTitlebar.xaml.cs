﻿using System;
using System.Collections.Generic;
using Drawing = System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;

namespace ESLTracker.Controls
{
    /// <summary>
    /// Interaction logic for OverlayWindowTitlebar.xaml
    /// </summary>
    public partial class OverlayWindowTitlebar : UserControl
    {



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(OverlayWindowTitlebar), new PropertyMetadata(String.Empty));



        public OverlayWindowTitlebar()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var window = this.FindParent<Window>();
            ((IOverlayWindow)window).ShowOnScreen = false;
        }

        private Brush originalBackgroundBrush;
        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb != null)
            {
                if (tb.IsChecked.Value)
                {
                    originalBackgroundBrush = ((StackPanel)Window.GetWindow(this).Content).Background;
                }
                ((StackPanel)Window.GetWindow(this).Content).Children[1].Visibility =
                    tb.IsChecked.Value ?  Visibility.Hidden : Visibility.Visible;
                ((StackPanel)Window.GetWindow(this).Content).Background =
                    tb.IsChecked.Value ? null : originalBackgroundBrush;
            }
            e.Handled = false;
        }

        private void btnScreenShot_Click(object sender, RoutedEventArgs e)
        {
            string fileName = new ScreenshotNameProvider().GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Regular);
            Task.Factory.StartNew(() => new FileManager(new TrackerFactory()).SaveScreenShot(fileName));
        }

        private void btnShowMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.WindowState == WindowState.Minimized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
            mainWindow.Focus();
        }
    }
}
