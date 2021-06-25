using Microsoft.Extensions.Logging;
using QuietTime.Other;
using QuietTime.Services;
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
        private readonly ILogger<MainWindow> _logger;
        private readonly ScheduleWindowVM svm;
        private readonly SettingsWindowVM _settingsVM;
        private readonly NotificationService _notifications;

        /// <summary>
        /// Creates a new <see cref="MainWindow"/>.
        /// </summary>
        /// <param name="vm">The viewmodel for this window.</param>
        /// <param name="svm">The viewmodel for the <see cref="ScheduleWindow"/> that can be opened from this window.</param>
        /// <param name="logger">Logging framework for this class.</param>
        /// <param name="notifications">Notification service for this class.</param>
        /// <param name="settingsVM">Viewmodel for the <see cref="SettingsWindow"/> that can be opened from this window.</param>
        public MainWindow(MainWindowVM vm, ScheduleWindowVM svm, ILogger<MainWindow> logger, NotificationService notifications, SettingsWindowVM settingsVM)
        {
            InitializeComponent();

            DataContext = vm;

            TrayIcon.Icon = new("icon.ico");
            this.svm = svm;
            _logger = logger;
            _notifications = notifications;
            _settingsVM = settingsVM;
        }

        // these all make the UI auto-scale when the window resizes
        private static readonly ScaleValueHelper<MainWindow> _scaleHelper = new();
        public readonly DependencyProperty ScaleValueProperty = _scaleHelper.Get();

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleValue = _scaleHelper.CalculateScale(ActualHeight, ActualWidth, MainGrid);
        }

        public double ScaleValue
        {
            get => (double)GetValue(ScaleValueProperty);
            set => SetValue(ScaleValueProperty, value);
        }

        // so we close to system tray by default

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !_traybarClosing;

            if (e.Cancel)
            {
                _notifications.SendNotification("Window closed",
                    "QuietTime is still running in the system tray. You can open the main window or close the app completely from there.",
                    NotificationService.MessageLevel.Information);

                _logger.LogInformation(EventIds.AppClosingCancelled, "App sent to system tray.");
            }
            else
            {
                _logger.LogInformation(EventIds.AppClosing, "App closed.");
            }

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
            this.Close();
        }

        // opening other windows

        private void Button_AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            new ScheduleWindow(svm).Show();
        }

        private void Button_OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow(_settingsVM).Show();
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
