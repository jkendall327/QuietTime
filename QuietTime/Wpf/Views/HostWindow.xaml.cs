using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.Logging;
using QuietTime.Core.Logging;
using QuietTime.Core.Notifications;
using QuietTime.Settings;
using QuietTime.Wpf.ViewModels;

namespace QuietTime.Wpf.Views
{
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    internal partial class HostWindow : Window
    {
        // services
        private readonly ILogger<HostWindow> _logger;
        private readonly INotifier _notifications;

        public HostWindow(HostViewModel viewModel, TrayIconVM trayViewModel, ILogger<HostWindow> logger, INotifier notifications)
        {
            InitializeComponent();

            _logger = logger;
            _notifications = notifications;

            DataContext = viewModel;
            TraybarIcon.DataContext = trayViewModel;

            trayViewModel.ShowAppRequested += (_, _) => this.Show();
            trayViewModel.CloseAppRequested += CloseApp;
        }

        private bool _closeToTray = true;

        private void CloseApp(object? sender, EventArgs e)
        {
            _logger.LogInformation(EventIds.AppClosing, "App closed.");
            _closeToTray = false;

            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!UserSettings.Default.MinimizeOnClose)
            {
                base.OnClosing(e); return;
            }

            e.Cancel = _closeToTray;

            if (e.Cancel)
            {
                _notifications.SendNotification("Window closed",
                    "QuietTime is still running in the system tray. You can re-open or close it from there.",
                    MessageLevel.Information);

                _logger.LogInformation(EventIds.AppClosingCancelled, "App sent to system tray.");

                this.Hide();

                return;
            }

            base.OnClosed(e);
        }
    }
}
