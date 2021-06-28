﻿using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuietTime.Other;
using QuietTime.Services;
using QuietTime.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace QuietTime
{
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ServiceProvider serviceProvider = await ConfigureServices(services);

            // let's go
            var main = serviceProvider.GetService<MainWindow>()!;

            if (e.Args.Length == 0 || e.Args[0] != "--minimized")
            {
                main.Show();
            }
        }

        private static async Task<ServiceProvider> ConfigureServices(ServiceCollection services)
        {
            // configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            services.Configure<Settings>(configuration.GetSection("Settings"));

            // logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.AddFile(configuration.GetSection("Logging"));
            });

            // register windows
            services.AddTransient<MainWindow>();
            services.AddTransient<MainWindowVM>();
            services.AddTransient<ScheduleWindowVM>();
            services.AddTransient<SettingsWindowVM>();

            // scheduler
            StdSchedulerFactory schedulerFactory = new();
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            services.AddSingleton<ScheduleJobFactory>();
            services.AddSingleton<IScheduler>(scheduler);
            services.AddTransient<SchedulerService>();

            // notifications
            services.AddSingleton<NotificationService>();
            services.AddSingleton<TaskbarIcon>();

            // audio
            services.AddTransient<MMDeviceEnumerator>();
            services.AddSingleton<AudioService>();

            // serialization
            services.AddTransient<SerializerService>();

            // autostart
            services.AddTransient<AutostartService>();

            // create service provider
            var provider = services.BuildServiceProvider();

            scheduler.JobFactory = provider.GetRequiredService<ScheduleJobFactory>();
            await scheduler.Start();

            return provider;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string message = "A serious error has occured and the program must now close. The next window will show details of the error that you can send to the developer.";

            MessageBox.Show(message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            var exceptionMessage = new List<string>() { DateTime.Now.ToString("G"), e.Exception.Message };

            File.AppendAllLines("error.log", exceptionMessage);
        }
    }

    public class ScheduleJobFactory : IJobFactory
    {
        private readonly NotificationService _notificationService;
        private readonly AudioService _audioService;
        private readonly ILogger<ChangeMaxVolumeJob> _logger;

        public ScheduleJobFactory(NotificationService notificationService, AudioService audioService, ILogger<ChangeMaxVolumeJob> logger)
        {
            _notificationService = notificationService;
            _audioService = audioService;
            _logger = logger;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return new ChangeMaxVolumeJob(_notificationService, _audioService, _logger, Application.Current.Dispatcher);
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
