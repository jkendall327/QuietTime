using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using QuietTime.Autostarting;
using QuietTime.Core.AudioLocking;
using QuietTime.Core.Scheduling;
using QuietTime.Navigation;
using QuietTime.Notifications;
using QuietTime.Serialization;
using QuietTime.Wpf.ViewModels;
using QuietTime.Wpf.Views;

namespace QuietTime
{
    /// <summary>
    /// Encapsulates setting up DI for the project.
    /// </summary>
    internal class ContainerProvider
    {
        public async Task<ServiceProvider> GetContainer()
        {
            var services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            services.AddSingleton(configuration)
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                    builder.AddFile(configuration.GetSection("Logging"));
                })
                .AddTransient<HostWindow>()
                .AddViewModels()
                .AddNotifications()
                .AddNavigation()
                .AddAudioLocking()
                .AddTransient<ISerializer, Serializer>()
                .AddTransient<Autostarter>();

            await services.AddScheduling();

            var provider = services.BuildServiceProvider();

            // clean-up, try to find a way to avoid this
            var scheduler = provider.GetRequiredService<IScheduler>();
            await scheduler.Start();

            return provider;
        }
    }
}