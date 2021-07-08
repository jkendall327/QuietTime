﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NAudio.CoreAudioApi;
using Quartz.Impl;
using Quartz;
using QuietTime.Services;
using QuietTime.ViewModels;
using QuietTime.Views;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace QuietTime.Other
{
    class DIContainerProvider
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
                .SetBasePath(Directory.GetCurrentDirectory())
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
            services.AddTransient<SchedulerService>();

            // notifications
            services.AddSingleton<TaskbarIcon>();
            services.AddSingleton<Dispatcher>(Application.Current.Dispatcher);
            services.AddSingleton<NotificationService>();

            // audio
            services.AddTransient<MMDeviceEnumerator>();
            services.AddSingleton<AudioService>();

            // serialization
            services.AddTransient<SerializerService>();

            // autostart
            services.AddTransient<AutostartService>();

            // create service provider
            var provider = services.BuildServiceProvider();

            // some final setup, todo change this...
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