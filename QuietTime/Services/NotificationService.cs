using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace QuietTime.Services
{
    /// <summary>
    /// Encapsulates sending notifications to the user.
    /// </summary>
    public class NotificationService
    {
        private readonly TaskbarIcon _tray;
        private readonly Settings _settings;
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Creates a new <see cref="NotificationService"/>.
        /// </summary>
        /// <param name="tray"></param>
        /// <param name="settings">Provides access to program settings.</param>
        public NotificationService(TaskbarIcon tray, IOptions<Settings> settings, Dispatcher dispatcher)
        {
            _settings = settings.Value;

            _tray = tray;
            _tray.Visibility = System.Windows.Visibility.Collapsed;
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Level of importance of notifications sent to user.
        /// </summary>
        public enum MessageLevel
        {
            /// <summary>
            /// Message has unclassified importance.
            /// </summary>
            None,

            /// <summary>
            /// Message indicates no issues with the system.
            /// </summary>
            Information,

            /// <summary>
            /// Message indicates a failure that the system can work through.
            /// </summary>
            Warning,

            /// <summary>
            /// Message indicates a catastrophic failure the system can not recover from.
            /// </summary>
            Error
        }

        /// <summary>
        /// Sends a notification to the user.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="message">The content of the message.</param>
        /// <param name="level">Indicates the relative importance of the message.</param>
        public void SendNotification(string title, string message, MessageLevel level)
        {
            if (!_settings.NotificationsEnabled) return;

            // have to do this because quartz.net can invoke this from another thread
            _dispatcher.Invoke(() => Send(title, message, level));
        }

        private void Send(string title, string message, MessageLevel level)
        {
            _tray.Visibility = System.Windows.Visibility.Visible;

            BalloonIcon icon = level switch
            {
                MessageLevel.None => BalloonIcon.None,
                MessageLevel.Information => BalloonIcon.Info,
                MessageLevel.Warning => BalloonIcon.Warning,
                MessageLevel.Error => BalloonIcon.Error,
                _ => BalloonIcon.None
            };

            _tray.ShowBalloonTip(title, message, BalloonIcon.None);

            _tray.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
