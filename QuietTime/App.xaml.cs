using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using QuietTime.Settings;
using QuietTime.Wpf.Views;

namespace QuietTime
{
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var services = await new ContainerProvider().GetContainer();

            var main = services.GetRequiredService<HostWindow>();

            if (!UserSettings.Default.LaunchMinimized)
            {
                main.Show();
            }
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
