using QuietTime.Other;
using QuietTime.Services;

namespace QuietTime.ViewModels
{
    /// <summary>
    /// Viewmodel for a settings page.
    /// </summary>
    internal class SettingsPageVM : ViewModelBase
    {
        public bool LaunchOnStartup
        {
            get { return _launchOnStartup; }
            set
            {
                SetProperty<bool>(ref _launchOnStartup, value);

                if (LaunchOnStartup)
                {
                    Autostarter.SetStartup();
                }
                else
                {
                    Autostarter.RemoveStartup();
                }
            }
        }
        private bool _launchOnStartup;

        public bool LaunchMinimized
        {
            get { return UserSettings.Default.LaunchMinimized; }
            set { UserSettings.Default.LaunchMinimized = value; UserSettings.Default.Save(); OnPropertyChanged(nameof(LaunchMinimized)); }
        }
        public bool EnableNotifications
        {
            get { return UserSettings.Default.NotificationsEnabled; }
            set { UserSettings.Default.NotificationsEnabled = value; UserSettings.Default.Save(); OnPropertyChanged(nameof(EnableNotifications)); }
        }
        public bool MinimizeOnClose
        {
            get { return UserSettings.Default.MinimizeOnClose; }
            set { UserSettings.Default.MinimizeOnClose = value; UserSettings.Default.Save(); OnPropertyChanged(nameof(MinimizeOnClose)); }
        }
        public bool LockOnStartup
        {
            get { return UserSettings.Default.LockOnStartup; }
            set { UserSettings.Default.LockOnStartup = value; UserSettings.Default.Save(); OnPropertyChanged(nameof(LockOnStartup)); }
        }

        public bool ActivateSchedulesOnCreation
        {
            get { return UserSettings.Default.ActivateSchedulesOnCreation; }
            set { UserSettings.Default.ActivateSchedulesOnCreation = value; UserSettings.Default.Save(); OnPropertyChanged(nameof(ActivateSchedulesOnCreation)); }
        }

        public int DefaultMaxVolume
        {
            get { return UserSettings.Default.DefaultMaxVolume; }
            set { UserSettings.Default.DefaultMaxVolume = value; UserSettings.Default.Save(); OnPropertyChanged(nameof(DefaultMaxVolume)); }
        }

        public Autostarter Autostarter { get; }

        public SettingsPageVM(Autostarter _autostart)
        {
            Autostarter = _autostart;
        }
    }
}
