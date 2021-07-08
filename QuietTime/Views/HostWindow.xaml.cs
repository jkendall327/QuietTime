using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuietTime.Other;
using QuietTime.Services;
using QuietTime.ViewModels;

namespace QuietTime.Views
{
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    public partial class HostWindow : Window
    {
        // services
        private readonly ILogger<HostWindow> _logger;
        private readonly NotificationService _notifications;

        public HostWindow(HostViewModel viewModel, TrayIconVM trayViewModel, ILogger<HostWindow> logger, NotificationService notifications)
        {
            InitializeComponent();

            _logger = logger;
            _notifications = notifications;

            DataContext = viewModel;
            TraybarIcon.DataContext = trayViewModel;

            viewModel.CloseAppRequested += CloseApp;
            trayViewModel.ShowAppRequested += (_, _) => this.Show();

            trayViewModel.CloseAppRequested += CloseApp;
            viewModel.HideAppRequested += (_, _) => this.Hide();
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

            _notifications.SendNotification("Window closed",
                "QuietTime is still running in the system tray. You can re-open or close it from there.",
                NotificationService.MessageLevel.Information);

            _logger.LogInformation(EventIds.AppClosingCancelled, "App sent to system tray.");

            this.Hide();
        }
    }
}
