using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Navigation;

namespace QuietTime.Wpf.ViewModels
{
    internal class NavigationBarVM
    {
        public ICommand NavigateCommand { get; set; }

        public MainPageVM HomeVM { get; set; }
        public ScheduleWindowVM ScheduleVM { get; set; }
        public SettingsPageVM SettingsVM { get; set; }

        public NavigationBarVM(Navigator navigator, MainPageVM homeVM, ScheduleWindowVM scheduleVM, SettingsPageVM settingsVM)
        {
            HomeVM = homeVM;
            ScheduleVM = scheduleVM;
            SettingsVM = settingsVM;

            // view just picks from one of the properties above to set the new VM
            // doesn't scale but it works alright for this
            NavigateCommand = new RelayCommand<ViewModelBase>((vm) =>
            {
                if (vm is not null) navigator.CurrentViewModel = vm;
            });
        }

        public string VersionInfo
        {
            get
            {
                // todo: this might not work right with single-file publish
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new FileInfo(AppContext.BaseDirectory).LastWriteTime;

                return $"QuietTime v{version}, built {buildDate:d}";
            }
        }
    }
}
