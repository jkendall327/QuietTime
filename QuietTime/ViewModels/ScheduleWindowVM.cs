using Microsoft.Extensions.Configuration;
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
    public class ScheduleWindowVM : ViewModelBase
    {
        private readonly SchedulerService _scheduler;
        private readonly SerializerService _serializer;
        private readonly NotificationService _notifier;

        /// <summary>
        /// Creates a new <see cref="ScheduleWindowVM"/>.
        /// </summary>
        /// <param name="scheduler">Used to pass schedules created here into the scheduling back-end.</param>
        /// <param name="serializer">Handles serialization of user's schedules.</param>
        public ScheduleWindowVM(SchedulerService scheduler, SerializerService serializer, NotificationService notifier)
        {
            _scheduler = scheduler;
            _serializer = serializer;
            _notifier = notifier;

            AddSchedule = new AsyncRelayCommand(AddScheduleAsync);

            Func<bool> anySchedules = Schedules.Any;
            DeactivateAll = new AsyncRelayCommand(DeactivateAllAsync, anySchedules);
            ActivateAll = new AsyncRelayCommand(ActivateAllAsync, anySchedules);

            Serialize = new AsyncRelayCommand(() => _serializer.SerializeSchedulesAsync(Schedules));

            Func<bool> scheduleIsSelected = () => SelectedSchedule is not null;
            DeleteSelected = new AsyncRelayCommand(RemoveScheduleAsync, scheduleIsSelected);
            FlipActivation = new AsyncRelayCommand(FlipActivationAsync, scheduleIsSelected);

            Commands = new() { FlipActivation, ActivateAll, DeactivateAll, DeleteSelected, Serialize };

            foreach (var schedule in _serializer.DeserializeSchedules())
            {
                Schedules.Add(schedule);
            }
        }

        private readonly List<IRelayCommand> Commands;

        /// <summary>
        /// The user's current schedules, both active and inactive.
        /// </summary>
        public ObservableCollection<Schedule> Schedules { get; private set; } = new();

        /// <summary>
        /// Currently-selected <see cref="Schedules"/>.
        /// </summary>
        public Schedule? SelectedSchedule
        {
            get { return _selectedSchedule; }
            set
            {
                _selectedSchedule = value;
                NotifyAllCommands();
            }
        }
        private Schedule? _selectedSchedule;

        private void NotifyAllCommands()
        {
            foreach (var command in Commands)
            {
                command.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Binding object for the UI.
        /// </summary>
        public Schedule NewSchedule
        {
            get { return _newSchedule; }
            set { SetProperty(ref _newSchedule, value); }
        }
        private Schedule _newSchedule = Schedule.Default;

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
            SelectedSchedule!.IsActive = !SelectedSchedule.IsActive;

            if (SelectedSchedule.Key is null) return;

            await _scheduler.FlipScheduleActivation(SelectedSchedule.Key);
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
            if (SelectedSchedule!.Key is not null)
            {
                await _scheduler.DeleteScheduleAsync(SelectedSchedule.Key);
            }

            Schedules.Remove(SelectedSchedule);
            NotifyAllCommands();
        }

        private async Task AddScheduleAsync()
        {
            var newSchedule = new Schedule(_newSchedule.Start, _newSchedule.End, _newSchedule.VolumeDuring, _newSchedule.VolumeAfter);

            if (Schedules.Any(x => x.Overlaps(newSchedule)))
            {
                _notifier.SendNotification("Error", "This schedule overlaps with an existing schedule.", NotificationService.MessageLevel.Error);
                return;
            }

            Schedules.Add(newSchedule);
            NotifyAllCommands();

            if (!UserSettings.Default.ActivateSchedulesOnCreation) return;

            var newJobKey = await _scheduler.CreateScheduleAsync(newSchedule);
            newSchedule.IsActive = true;
            newSchedule.Key = newJobKey;
        }
    }
}
