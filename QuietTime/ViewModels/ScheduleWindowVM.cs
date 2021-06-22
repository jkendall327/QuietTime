using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Models;
using QuietTime.Other;
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
        public ObservableCollection<Schedule> Schedules { get; private set; } = new ();

        private int _currentIndex;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
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

        public RelayCommand AddSchedule { get; set; }
        public RelayCommand FlipActivation { get; set; }
        public RelayCommand ActivateAll { get; set; }
        public RelayCommand DeactivateAll { get; set; }
        public RelayCommand DeleteSelected { get; set;}

        private readonly SchedulerService _scheduler;

        public ScheduleWindowVM(SchedulerService scheduler)
        {
            _scheduler = scheduler;

            CreateCommands();

            foreach (var item in Schedule.GetSchedules())
            {
                Schedules.Add(item);
            }
        }

        private void CreateCommands()
        {
            AddSchedule = new RelayCommand(async () => await Add());

            DeleteSelected = new RelayCommand(() => Schedules.Remove(Schedules[CurrentIndex]));

            ActivateAll = new RelayCommand(() =>
            {
                foreach (var item in Schedules.Where(x => !x.IsActive))
                {
                    item.IsActive = true;
                }
            });

            DeactivateAll = new RelayCommand(() =>
            {
                foreach (var item in Schedules)
                {
                    item.IsActive = false;
                }
            });

            FlipActivation = new RelayCommand(() =>
            {
                var item = Schedules[CurrentIndex];
                item.IsActive = !item.IsActive;
            });
        }

        private async Task Add()
        {
            var schedule = new Schedule(
                TimeOnly.FromDateTime(Start), 
                TimeOnly.FromDateTime(End), 
                VolumeDuring, 
                VolumeAfter);

            Schedule.AddSchedule(schedule);

            Schedules.Add(schedule);

            schedule.key = await _scheduler.CreateScheduleAsync(schedule);
        }
    }
}
