using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using Quartz;
using Quartz.Impl;
using QuietTime.Other;
using QuietTime.Services;
using QuietTime.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace QuietTime
{
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();

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

            // scheduler
            StdSchedulerFactory schedulerFactory = new();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            services.AddSingleton<IScheduler>(scheduler);
            services.AddTransient<SchedulerService>();

            // notifications
            services.AddTransient<NotificationService>();
            services.AddSingleton<TaskbarIcon>();

            // audio
            services.AddTransient<MMDeviceEnumerator>();
            services.AddTransient<AudioService>();

            // serialization
            services.AddTransient<SerializerService>();

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // let's go
            serviceProvider.GetService<MainWindow>()!.Show();
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
}
