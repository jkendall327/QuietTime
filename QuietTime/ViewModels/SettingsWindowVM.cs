using IWshRuntimeLibrary;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Other;
using QuietTime.Services;
using System;
using System.IO;
using File = System.IO.File;

namespace QuietTime.ViewModels
{
    /// <summary>
    /// Viewmodel for a settings window.
    /// </summary>
    public class SettingsWindowVM : ObservableObject
    {
        /// <summary>
        /// Whether QuietTime automatically launches when the user signs in.
        /// </summary>
        public bool LaunchOnStartup
        {
            get { return _launchOnStartup; }
            set { _launchOnStartup = value; }
        }
        private bool _launchOnStartup;

        /// <summary>
        /// Whether QuietTime will launch minimized in the system tray.
        /// </summary>
        public bool LaunchMinimized
        {
            get { return _launchMinimized; }
            set { _launchMinimized = value; }
        }
        private bool _launchMinimized;

        /// <summary>
        /// Either creates or delete the shortcut in the user's startup folder.
        /// </summary>
        public RelayCommand SwitchLaunchOptions { get; set; }

        private readonly NotificationService _notifications;
        private readonly ILogger<SettingsWindowVM> _logger;

        /// <summary>
        /// Creates a new <see cref="SettingsWindowVM"/>.
        /// </summary>
        /// <param name="notifications">Notification service for this class.</param>
        /// <param name="logger">Logging service for this class.</param>
        public SettingsWindowVM(NotificationService notifications, ILogger<SettingsWindowVM> logger)
        {
            _notifications = notifications;
            _logger = logger;

            SwitchLaunchOptions = new RelayCommand(() =>
            {
                if (LaunchOnStartup)
                {
                    SetStartup();
                }
                else
                {
                    RemoveStartup();
                }
            });
        }

        private string ShortcutPath()
        {
            var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return Path.Combine(startupFolderPath, @"QuietTime.lnk");
        }

        private void SetStartup()
        {
            var shortcutPath = ShortcutPath();

            if (File.Exists(shortcutPath))
            {
                return;
            }

            var shell = new WshShell();
            var sc = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            if (LaunchMinimized)
            {
                sc.Arguments = "--minimized";
            }

            sc.Description = "QuietTime";
            sc.WorkingDirectory = Directory.GetCurrentDirectory();
            sc.TargetPath = Path.Combine(sc.WorkingDirectory, @"QuietTime.exe");

            sc.Save();

            _logger.LogInformation(EventIds.AutomaticStartupAdded, "Added shortcut to shell:startup.");

            _notifications.SendNotification("Start-up",
                "QuietTime will now automatically start when you sign in. This won't affect other users. This added a shortcut file to your Startup folder.",
                NotificationService.MessageLevel.Information);
        }

        private void RemoveStartup()
        {
            var shortcutPath = ShortcutPath();

            if (!File.Exists(shortcutPath)) return;

            try
            {
                File.Delete(shortcutPath);

                _logger.LogInformation(EventIds.AutomaticStartupRemoved, "Removed shortcut from shell:startup.");

                _notifications.SendNotification("Start-up",
                "QuietTime will no longer automatically start when you sign in. This removed a shortcut file in your Startup folder.",
                NotificationService.MessageLevel.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.AutomaticStartupRemoved, ex, "Exception when removing shortcut from shell:startup.");

                _notifications.SendNotification("Error",
                    "An error ocurred when deleting QuietTime's shortcut. You can try again or delete the shortcut manually to stop the program from automatically starting. Enter 'shell:startup' in Windows Explorer to find it.",
                    NotificationService.MessageLevel.Error);
            }
        }
    }
}
