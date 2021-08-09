using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using QuietTime.Core.Notifications;

namespace QuietTime.Notifications
{
    public static class NotifierRegistrationExtensions
    {
        public static IServiceCollection AddNotifications(this IServiceCollection services)
        {
            services.AddSingleton<TaskbarIcon>()
            .AddSingleton(Application.Current.Dispatcher)
            .AddSingleton<INotifier, Notifier>();

            return services;
        } 
    }
}