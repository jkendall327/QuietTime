using Microsoft.Extensions.Logging;
using QuietTime.Other;
using QuietTime.Services;
using QuietTime.ViewModels;
using QuietTime.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace QuietTime
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // services
        private readonly ILogger<MainWindow> _logger;
        private readonly NotificationService _notifications;

        // we have these variables just so we can close all windows when main window closes
        ScheduleWindow? _schedules;
        SettingsWindow? _settings;

        /// <summary>
        /// Creates a new <see cref="MainWindow"/>.
        /// </summary>
        /// <param name="viewModel">The viewmodel for this window.</param>
        /// <param name="schedulesViewModel">The viewmodel for the <see cref="ScheduleWindow"/> that can be opened from this window.</param>
        /// <param name="logger">Logging framework for this class.</param>
        /// <param name="notifications">Notification service for this class.</param>
        /// <param name="settingsViewModel">Viewmodel for the <see cref="SettingsWindow"/> that can be opened from this window.</param>
        public MainWindow(MainWindowVM viewModel, ScheduleWindowVM schedulesViewModel, ILogger<MainWindow> logger, NotificationService notifications, SettingsWindowVM settingsViewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            TrayIcon.Icon = new("icon.ico");
            _logger = logger;
            _notifications = notifications;

            // open windows
            ScheduleWindowButton.Click += (s, e) =>
            {
                _schedules = new ScheduleWindow(schedulesViewModel);
                _schedules.Show();
            };

            SettingsWindowButton.Click += (s, e) =>
            {
                _settings = new SettingsWindow(settingsViewModel);
                _settings.Show();
            };

            // tray icon menus
            ShowWindowMenu.Click += (s, e) => this.Show();
            CloseAppMenu.Click += (s, e) =>
            {
                _traybarClosing = true;
                this.Close();
            };
        }

        // make the UI auto-scale when the window resizes
        private static readonly ScaleValueHelper<MainWindow> _scaleHelper = new();
        private readonly DependencyProperty ScaleValueProperty = _scaleHelper.Get();

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleValue = _scaleHelper.CalculateScale(ActualHeight, ActualWidth, MainGrid);
        }

        public double ScaleValue
        {
            get => (double)GetValue(ScaleValueProperty);
            set => SetValue(ScaleValueProperty, value);
        }

        // close to system tray by default
        private bool _traybarClosing = false;

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
                // app won't exit if other windows are open
                _settings?.Close();
                _schedules?.Close();

                _logger.LogInformation(EventIds.AppClosing, "App closed.");
            }

            this.Hide();

            base.OnClosing(e);
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
