using QuietTime.Core.Other;

namespace QuietTime.Core.Services
{
    /// <summary>
    /// Encapsulates sending notifications to the user.
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// Sends a notification to the user.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="message">The content of the message.</param>
        /// <param name="level">Indicates the relative importance of the message.</param>
        void SendNotification(string title, string message, MessageLevel level);
    }
}