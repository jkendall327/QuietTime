using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using Quartz;
using Quartz.Impl;
using QuietTime.Other;
using QuietTime.Services;
using QuietTime.ViewModels;
using System.Windows;

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
    }
}
