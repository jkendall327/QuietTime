using IWshRuntimeLibrary;
using Microsoft.Extensions.Logging;
using QuietTime.Other;
using QuietTime.ViewModels;
using System;
using System.IO;
using File = System.IO.File;

namespace QuietTime.Services
{
    /// <summary>
    /// Encapsulates letting QuietTime automatically start on sign-in.
    /// </summary>
    public class AutostartService
    {
        private readonly NotificationService _notifications;
        private readonly ILogger<SettingsWindowVM> _logger;

        /// <summary>
        /// Creates a new <see cref="SettingsWindowVM"/>.
        /// </summary>
        /// <param name="notifications">Notification service for this class.</param>
        /// <param name="logger">Logging service for this class.</param>
        public AutostartService(NotificationService notifications, ILogger<SettingsWindowVM> logger)
        {
            _notifications = notifications;
            _logger = logger;
        }

        private string ShortcutPath()
        {
            var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return Path.Combine(startupFolderPath, @"QuietTime.lnk");
        }

        /// <summary>
        /// Sets program to start when user signs-in.
        /// </summary>
        /// <param name="launchMinimized"></param>
        public void SetStartup(bool launchMinimized)
        {
            var shortcutPath = ShortcutPath();

            if (File.Exists(shortcutPath)) return;

            var shell = new WshShell();
            var sc = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            if (launchMinimized)
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

        /// <summary>
        /// Stops program from starting when user signs-in.
        /// </summary>
        public void RemoveStartup()
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
