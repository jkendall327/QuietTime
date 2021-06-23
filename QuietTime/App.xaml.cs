using Autofac;
using Microsoft.Extensions.Configuration;
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
            var builder = new ContainerBuilder();

            // windows
            builder.RegisterType<MainWindowVM>();
            builder.RegisterType<ScheduleWindowVM>();
            builder.RegisterType<MainWindow>();

            // configuration
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("config.json").Build();
            builder.RegisterInstance(config);

            // logging
            var factory = LoggerFactory.Create(x =>
            {
                x.AddConsole();
                x.AddDebug();
                x.AddFile(config.GetSection("Logging"));
            });

            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>));
            builder.RegisterInstance(factory);

            // scheduling
            StdSchedulerFactory schedulerFactory = new();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            builder.RegisterInstance(scheduler);
            builder.RegisterType<SchedulerService>();

            // audio
            builder.RegisterInstance(new MMDeviceEnumerator());
            builder.RegisterType<AudioService>();

            // let's go
            builder.Build().Resolve<MainWindow>().Show();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("A serious error has occured and the program must now close. The next window will show details of the error that you can send to the developer.", "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            var message = new List<string>() { DateTime.Now.ToString("G"), e.Exception.Message };

            File.AppendAllLines("error.log", message);
        }
    }
}
