using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using Quartz;
using Quartz.Impl;
using QuietTime.Core.Services;
using QuietTime.Core.Services.Scheduling;
using QuietTime.Services;
using QuietTime.ViewModels;
using QuietTime.Views;

namespace QuietTime.Other
{
    /// <summary>
    /// Encapsulates setting up DI for the project.
    /// </summary>
    internal class DIContainerProvider
    {
        public async Task<ServiceProvider> GetContainer()
        {
            var services = new ServiceCollection();

            RegisterHostServices(services);

            RegisterPages(services);

            return await RegisterServices(services);
        }

        private void RegisterHostServices(ServiceCollection services)
        {
            // configuration
            IConfiguration configuration = new ConfigurationBuilder()
                // using Directory.GetCurrentDirectory() here breaks when launching from shortcut file
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.AddFile(configuration.GetSection("Logging"));
            });
        }

        private async Task<ServiceProvider> RegisterServices(ServiceCollection services)
        {
            // scheduling
            StdSchedulerFactory schedulerFactory = new();
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            services.AddSingleton<ScheduleJobFactory>();
            services.AddSingleton<IScheduler>(scheduler);
            services.AddTransient<ISchedulingService, Scheduler>();

            // notifications
            services.AddSingleton<TaskbarIcon>();
            services.AddSingleton<Dispatcher>(Application.Current.Dispatcher);

            services.AddSingleton<INotifier, Notifier>();

            // audio
            services.AddTransient<MMDeviceEnumerator>();
            services.AddSingleton<IAudioLocker, AudioLocker>();

            // serialization
            services.AddTransient<ISerializer, Serializer>();

            // autostart
            services.AddTransient<Autostarter>();

            // create service provider
            var provider = services.BuildServiceProvider();

            // todo: some final setup, change this...
            scheduler.JobFactory = provider.GetRequiredService<ScheduleJobFactory>();
            await scheduler.Start();

            return provider;
        }

        private void RegisterPages(ServiceCollection services)
        {
            services.AddTransient<HostWindow>();
            services.AddTransient<HostViewModel>();

            // navigation
            services.AddTransient<NavigationBarVM>();
            services.AddSingleton<Navigator>(s => new Navigator()
            {
                CurrentViewModel = s.GetRequiredService<MainPageVM>(),
            });

            services.AddTransient<TrayIconVM>();

            services.AddTransient<MainPageVM>();
            services.AddTransient<ScheduleWindowVM>();
            services.AddTransient<SettingsPageVM>();
        }
    }
}
