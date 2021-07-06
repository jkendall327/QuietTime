using IWshRuntimeLibrary;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Other;
using QuietTime.Services;
using System;
using System.IO;
using File = System.IO.File;

namespace QuietTime.ViewModels
{
    /// <summary>
    /// Viewmodel for a settings window.
    /// </summary>
    public class SettingsWindowVM : ViewModelBase
    {
        /// <summary>
        /// Whether QuietTime automatically launches when the user signs in.
        /// </summary>
        public bool LaunchOnStartup
        {
            get { return _launchOnStartup; }
            set { _launchOnStartup = value; }
        }
        private bool _launchOnStartup;

        /// <summary>
        /// Whether QuietTime will launch minimized in the system tray.
        /// </summary>
        public bool LaunchMinimized
        {
            get { return _launchMinimized; }
            set { _launchMinimized = value; }
        }
        private bool _launchMinimized;

        /// <summary>
        /// Either creates or delete the shortcut in the user's startup folder.
        /// </summary>
        public RelayCommand SwitchLaunchOptions { get; set; }

        /// <summary>
        /// Creates a new <see cref="SettingsWindowVM"/>.
        /// </summary>
        public SettingsWindowVM(AutostartService _autostart)
        {
            SwitchLaunchOptions = new RelayCommand(() =>
            {
                if (LaunchOnStartup)
                {
                    _autostart.SetStartup(LaunchMinimized);
                }
                else
                {
                    _autostart.RemoveStartup();
                }
            });
        }
    }
}
