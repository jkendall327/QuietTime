﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Models;
using QuietTime.Other;
using QuietTime.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuietTime.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="QuietTime.Views.ScheduleWindow"/>.
    /// </summary>
    public class ScheduleWindowVM : ObservableObject
    {
        private readonly SchedulerService _scheduler;
        private readonly SerializerService _serializer;

        /// <summary>
        /// Creates a new <see cref="ScheduleWindowVM"/>.
        /// </summary>
        /// <param name="scheduler">Used to pass schedules created here into the scheduling back-end.</param>
        /// <param name="serializer">Handles serialization of user's schedules.</param>
        public ScheduleWindowVM(SchedulerService scheduler, SerializerService serializer)
        {
            _scheduler = scheduler;
            _serializer = serializer;

            AddSchedule = new AsyncRelayCommand(AddScheduleAsync);
            DeleteSelected = new AsyncRelayCommand(RemoveScheduleAsync);
            DeactivateAll = new AsyncRelayCommand(DeactivateAllAsync);
            FlipActivation = new AsyncRelayCommand(FlipActivationAsync);
            ActivateAll = new AsyncRelayCommand(ActivateAllAsync);

            Serialize = new AsyncRelayCommand(() => _serializer.SerializeSchedulesAsync(Schedules));

            foreach (var schedule in _serializer.DeserializeSchedules())
            {
                Schedules.Add(schedule);
            }
        }

        /// <summary>
        /// The user's current schedules, both active and inactive.
        /// </summary>
        public static ObservableCollection<Schedule> Schedules { get; private set; } = new();

        /// <summary>
        /// Currently-selected index of <see cref="Schedules"/>.
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
        }
        private int _currentIndex;

        /// <summary>
        /// Binding object for the UI.
        /// </summary>
        public Schedule Schedule
        {
            get { return _schedule; }
            set { SetProperty(ref _schedule, value); }
        }
        private Schedule _schedule = Schedule.Default;

        /// <summary>
        /// Creates a schedule.
        /// </summary>
        public AsyncRelayCommand AddSchedule { get; set; }

        /// <summary>
        /// Pauses active schedules and activates paused schedules.
        /// </summary>
        public AsyncRelayCommand FlipActivation { get; set; }

        /// <summary>
        /// Activates all schedules.
        /// </summary>
        public AsyncRelayCommand ActivateAll { get; set; }

        /// <summary>
        /// Deactivates all schedules.
        /// </summary>
        public AsyncRelayCommand DeactivateAll { get; set; }

        /// <summary>
        /// Deletes the schedule indicated by the <see cref="CurrentIndex"/>.
        /// </summary>
        public AsyncRelayCommand DeleteSelected { get; set; }

        /// <summary>
        /// Serializes the current contents of <see cref="Schedules"/> to JSON.
        /// </summary>
        public AsyncRelayCommand Serialize { get; set; }

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

            if (item.Key is null) return;

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

            if (item.Key is null) return;

            await _scheduler.DeleteScheduleAsync(item.Key);
        }

        private async Task AddScheduleAsync()
        {
            var newSchedule = new Schedule(_schedule.Start, _schedule.End, _schedule.VolumeDuring, _schedule.VolumeAfter);

            if (Schedules.Any(x => x.Overlaps(newSchedule)))
            {
                return;
            }

            Schedules.Add(newSchedule);

            var newJobKey = await _scheduler.CreateScheduleAsync(newSchedule);
            
            newSchedule.Key = newJobKey;
            newSchedule.IsActive = true;
        }
    }
}
