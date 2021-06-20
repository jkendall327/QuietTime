using Autofac;
using Microsoft.Extensions.Configuration;
using NAudio.CoreAudioApi;
using QuietTime.ViewModels;
using System.Windows;

namespace QuietTime
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("config.json").Build();

            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindowVM>();
            builder.RegisterType<MainWindow>();
            builder.RegisterInstance(new MMDeviceEnumerator());
            builder.RegisterInstance(config);

            builder.Build().Resolve<MainWindow>().Show();
        }
    }
}
