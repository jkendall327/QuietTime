using Autofac;
using NAudio.CoreAudioApi;
using QuietTime.ViewModels;
using System.Windows;

namespace QuietTime
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindowVM>();
            builder.RegisterType<MainWindow>();
            builder.RegisterInstance(new MMDeviceEnumerator());

            builder.Build().Resolve<MainWindow>().Show();
        }
    }
}
