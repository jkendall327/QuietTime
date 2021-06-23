﻿using Hardcodet.Wpf.TaskbarNotification;
using QuietTime.ViewModels;
using QuietTime.Views;
using System;
using System.ComponentModel;
using System.Windows;

namespace QuietTime
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ScheduleWindowVM svm;

        /// <summary>
        /// Creates a new <see cref="MainWindow"/>.
        /// </summary>
        /// <param name="vm">The viewmodel for this window.</param>
        /// <param name="svm">The viewmodel for the <see cref="ScheduleWindow"/> that can be opened from this window.</param>
        public MainWindow(MainWindowVM vm, ScheduleWindowVM svm)
        {
            InitializeComponent();

            DataContext = vm;

            TrayIcon.Icon = new("icon.ico");
            this.svm = svm;
        }

        /*
         * Source for scaling UI: https://www.inchoatethoughts.com/scaling-your-user-interface-in-a-wpf-application
         * I don't fully understand this, I'm not sure I want to fully understand this, but it seems to work OK.
         */

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            double yScale = ActualHeight / 400.0d;
            double xScale = ActualWidth / 500.0d;
            double value = Math.Min(xScale, yScale);
            ScaleValue = (double)OnCoerceScaleValue(MainGrid, value);
        }

        #region ScaleValue Depdency Property

        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register(
            "ScaleValue", typeof(double), typeof(MainWindow),
            new UIPropertyMetadata(
                1.0,
                new PropertyChangedCallback(OnScaleValueChanged),
                new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            if (o is MainWindow mainWindow)
            {
                return mainWindow.OnCoerceScaleValue((double)value);
            }

            return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0d;

            return Math.Max(0.1, value);
        }

        public double ScaleValue
        {
            get => (double)GetValue(ScaleValueProperty);
            set => SetValue(ScaleValueProperty, value);
        }

        #endregion

        // these two methods let the app close to the system tray instead of exiting completely

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !_traybarClosing;

            this.Hide();

            base.OnClosing(e);
        }

        private void MenuItem_ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private bool _traybarClosing = false;

        private void MenuItem_CloseApp_Click(object sender, RoutedEventArgs e)
        {
            _traybarClosing = true;
            Environment.Exit(1);
        }

        private void Button_AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            new ScheduleWindow(svm).Show();
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
