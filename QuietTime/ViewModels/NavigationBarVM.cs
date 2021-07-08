using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Services;

namespace QuietTime.ViewModels
{
    public class NavigationBarVM
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

            NavigateCommand = new RelayCommand<ViewModelBase>((vm) =>
            {
                if (vm is not null) navigator.CurrentViewModel = vm;
            });
        }

        public string VersionInfo
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new FileInfo(AppContext.BaseDirectory).LastWriteTime;

                return $"QuietTime v{version}, built {buildDate:d}";
            }
        }
    }
}
