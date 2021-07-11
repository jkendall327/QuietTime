using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Options;
using QuietTime.Core.Other;
using QuietTime.Core.Services;
using QuietTime.Other;
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
    public class Notifier : INotifier
    {
        // there's better ways than using an invisible taskbar icon, but it works for now
        private readonly TaskbarIcon _tray;
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Creates a new <see cref="Notifier"/>.
        /// </summary>
        /// <param name="tray"></param>
        /// <param name="settings">Provides access to program settings.</param>
        public Notifier(TaskbarIcon tray, Dispatcher dispatcher)
        {
            _tray = tray;
            _tray.Visibility = System.Windows.Visibility.Collapsed;
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Sends a notification to the user.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="message">The content of the message.</param>
        /// <param name="level">Indicates the relative importance of the message.</param>
        public void SendNotification(string title, string message, MessageLevel level)
        {
            if (!UserSettings.Default.NotificationsEnabled) return;

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
