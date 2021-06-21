using Microsoft.Toolkit.Mvvm.ComponentModel;
using QuietTime.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietTime.ViewModels
{
    public class ScheduleWindowVM : ObservableObject
    {
        private ObservableCollection<Schedule> _schedules = new();
        public ObservableCollection<Schedule> Schedules
        {
            get { return _schedules; }
            set { _schedules = value; }
        }

        public ScheduleWindowVM()
        {
            foreach (var item in Schedule.GetSchedules())
            {
                Schedules.Add(item);
            }
        }
    }
}
