using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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

        private DateTime _start;
        public DateTime Start
        {
            get { return _start; }
            set { SetProperty(ref _start, value); }
        }

        private DateTime _end;
        public DateTime End
        {
            get { return _end; }
            set { SetProperty(ref _end, value); }
        }

        private int _volumeDuring;

        public int VolumeDuring
        {
            get { return _volumeDuring; }
            set { SetProperty(ref _volumeDuring, value); }
        }

        private int _volumeAfter;

        public int VolumeAfter
        {
            get { return _volumeAfter; }
            set { SetProperty(ref _volumeAfter, value); }
        }

        private RelayCommand _addSchedule;
        public RelayCommand AddSchedule
        {
            get { return _addSchedule; }
            set { SetProperty(ref _addSchedule, value); }
        }

        public ScheduleWindowVM()
        {
            _addSchedule = new RelayCommand(Add);

            foreach (var item in Schedule.GetSchedules())
            {
                Schedules.Add(item);
            }
        }

        private void Add()
        {
            var schedule = new Schedule(
                TimeOnly.FromDateTime(Start), 
                TimeOnly.FromDateTime(End), 
                VolumeDuring, 
                VolumeAfter);

            Schedule.AddSchedule(schedule);

            Schedules.Clear();
            foreach (var item in Schedule.GetSchedules())
            {
                Schedules.Add(item);
            }
        }
    }
}
