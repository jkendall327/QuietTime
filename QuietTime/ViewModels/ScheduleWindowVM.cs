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
        public static ObservableCollection<Schedule> Schedules { get; private set; } = new();

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

        public AsyncRelayCommand AddSchedule { get; set; }
        public AsyncRelayCommand FlipActivation { get; set; }
        public AsyncRelayCommand ActivateAll { get; set; }
        public AsyncRelayCommand DeactivateAll { get; set; }
        public AsyncRelayCommand DeleteSelected { get; set;}

        private readonly SchedulerService _scheduler;

        public ScheduleWindowVM(SchedulerService scheduler)
        {
            _scheduler = scheduler;

            AddSchedule = new AsyncRelayCommand(AddScheduleAsync);
            DeleteSelected = new AsyncRelayCommand(RemoveScheduleAsync);
            DeactivateAll = new AsyncRelayCommand(DeactivateAllAsync);
            FlipActivation = new AsyncRelayCommand(FlipActivationAsync);
            ActivateAll = new AsyncRelayCommand(ActivateAllAsync);

#if DEBUG
            Schedules = new()
            {
                new Schedule(TimeOnly.Parse("09:00"), TimeOnly.Parse("17:45"), 20, 40),
                new Schedule(TimeOnly.Parse("11:00"), TimeOnly.Parse("15:00"), 10, 50),
                new Schedule(TimeOnly.Parse("03:00"), TimeOnly.Parse("12:00"), 23, 70),
            };
#endif
        }
        private async Task ActivateAllAsync()
        {
            foreach (var item in Schedules.Where(x => !x.IsActive))
            {
                item.IsActive = true;
            }

            await _scheduler.ResumeAll();
        }

        private async Task FlipActivationAsync()
        {
            var item = Schedules[CurrentIndex];

            item.IsActive = !item.IsActive;
            await _scheduler.FlipScheduleActivation(item.Key);
        }

        private async Task DeactivateAllAsync()
        {
            foreach (var item in Schedules)
            {
                item.IsActive = false;
            }

            await _scheduler.PauseAll();
        }

        private async Task RemoveScheduleAsync()
        {
            var item = Schedules[CurrentIndex];
            Schedules.Remove(item);
            await _scheduler.DeleteScheduleAsync(item.Key);
        }

        private async Task AddScheduleAsync()
        {
            var newSchedule = new Schedule(
                TimeOnly.FromDateTime(Start), 
                TimeOnly.FromDateTime(End), 
                VolumeDuring, 
                VolumeAfter);

            if (Schedules.Any(x => x.Overlaps(newSchedule)))
            {
                return;
            }

            Schedules.Add(newSchedule);
            Schedules.Add(newSchedule);
            newSchedule.Key = await _scheduler.CreateScheduleAsync(newSchedule);
        }
    }
}
