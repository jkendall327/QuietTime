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
using QuietTime.Views;
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
            var services = await new DIContainerProvider().GetContainer();

            var main = services.GetRequiredService<HostWindow>();

            if (e.Args.Length == 0 || e.Args[0] != "--minimized")
                main.Show();
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
