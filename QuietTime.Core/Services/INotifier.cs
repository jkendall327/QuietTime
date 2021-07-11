using QuietTime.Core.Other;

namespace QuietTime.Core.Services
{
    public interface INotifier
    {
        void SendNotification(string title, string message, MessageLevel level);
    }
}