using System;
using System.IO;
using Microsoft.Extensions.Logging;
using QuietTime.Core.Other;
using QuietTime.Core.Services;
using QuietTime.ViewModels;
using ShellLink;
using File = System.IO.File;

namespace QuietTime.Services
{
    /// <summary>
    /// Encapsulates letting QuietTime automatically start on sign-in.
    /// </summary>
    internal class Autostarter
    {
        private readonly INotifier _notifications;
        private readonly ILogger<SettingsPageVM> _logger;

        /// <summary>
        /// Creates a new <see cref="SettingsPageVM"/>.
        /// </summary>
        /// <param name="notifications">Notification service for this class.</param>
        /// <param name="logger">Logging service for this class.</param>
        public Autostarter(INotifier notifications, ILogger<SettingsPageVM> logger)
        {
            _notifications = notifications;
            _logger = logger;
        }

        private string ShortcutPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), @"QuietTime.lnk");
        private string AppLocation => Path.Combine(AppContext.BaseDirectory, @"QuietTime.exe");

        /// <summary>
        /// Sets program to start when user signs-in.
        /// </summary>
        public void SetStartup()
        {
            Shortcut.CreateShortcut(AppLocation).WriteToFile(ShortcutPath);

            _logger.LogInformation(EventIds.AutomaticStartupAdded, "Added shortcut to shell:startup.");

            _notifications.SendNotification("Start-up",
                "QuietTime will now automatically start when you sign in. This won't affect other users. This added a shortcut file to your Startup folder.",
                MessageLevel.Information);
        }

        /// <summary>
        /// Stops program from starting when user signs-in.
        /// </summary>
        public void RemoveStartup()
        {
            if (!File.Exists(ShortcutPath)) return;

            try
            {
                File.Delete(ShortcutPath);

                _logger.LogInformation(EventIds.AutomaticStartupRemoved, "Removed shortcut from shell:startup.");

                _notifications.SendNotification("Start-up",
                "QuietTime will no longer automatically start when you sign in. This removed a shortcut file in your Startup folder.",
                MessageLevel.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.AutomaticStartupRemoved, ex, "Exception when removing shortcut from shell:startup.");

                _notifications.SendNotification("Error",
                    "An error ocurred when deleting QuietTime's shortcut. You can delete the shortcut manually to stop it automatically starting. Enter 'shell:startup' in Windows Explorer to find it.",
                    MessageLevel.Error);
            }
        }
    }
}
