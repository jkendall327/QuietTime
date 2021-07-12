using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Core.Models;
using QuietTime.Core.Other;
using QuietTime.Core.Services;
using QuietTime.Core.Services.Scheduling;
using QuietTime.Other;
using QuietTime.Services;

namespace QuietTime.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="QuietTime.Views.ScheduleWindow"/>.
    /// </summary>
    internal class ScheduleWindowVM : ViewModelBase
    {
        private readonly ISchedulingService _scheduler;
        private readonly ISerializer _serializer;
        private readonly INotifier _notifier;

        /// <summary>
        /// Creates a new <see cref="ScheduleWindowVM"/>.
        /// </summary>
        /// <param name="scheduler">Used to pass schedules created here into the scheduling back-end.</param>
        /// <param name="serializer">Handles serialization of user's schedules.</param>
        public ScheduleWindowVM(ISchedulingService scheduler, ISerializer serializer, INotifier notifier)
        {
            _scheduler = scheduler;
            _serializer = serializer;
            _notifier = notifier;

            // set up commands
            AddSchedule = new AsyncRelayCommand(AddScheduleAsync);

            Func<bool> anySchedules = Schedules.Any;
            DeactivateAll = new AsyncRelayCommand(DeactivateAllAsync, anySchedules);
            ActivateAll = new AsyncRelayCommand(ActivateAllAsync, anySchedules);

            Serialize = new AsyncRelayCommand(() => _serializer.SerializeSchedulesAsync(Schedules));

            Func<bool> scheduleIsSelected = () => SelectedSchedule is not null;
            DeleteSelected = new AsyncRelayCommand(RemoveScheduleAsync, scheduleIsSelected);
            FlipActivation = new AsyncRelayCommand(FlipActivationAsync, scheduleIsSelected);

            // so we can call NotifyCanExecuteChanged() on them all easily
            Commands = new() { FlipActivation, ActivateAll, DeactivateAll, DeleteSelected, Serialize };

            // get user's schedules
            foreach (var schedule in _serializer.DeserializeSchedules())
            {
                Schedules.Add(schedule);
            }
        }

        /* Simplify telling all commands to update their CanExecute() */

        private readonly List<IRelayCommand> Commands;
        private void NotifyAllCommands()
        {
            foreach (var command in Commands)
            {
                command.NotifyCanExecuteChanged();
            }
        }

        /* UI Bindings */

        public ObservableCollection<Schedule> Schedules { get; } = new();

        /// <summary>
        /// The currently-selected <see cref="Schedules"/>.
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

        /// <summary>
        /// Schedule being created by user.
        /// </summary>
        public Schedule NewSchedule
        {
            get { return _newSchedule; }
            set { SetProperty(ref _newSchedule, value); }
        }
        private Schedule _newSchedule = Schedule.Default;

        /* Commands */

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
        /// Serializes the current contents of <see cref="Schedules"/>.
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
            Schedule newSchedule = new()
            {
                Start = _newSchedule.Start,
                End = _newSchedule.End,
                VolumeDuring = _newSchedule.VolumeDuring,
                VolumeAfter = _newSchedule.VolumeAfter,
            };

            if (Schedules.Any(x => x.Overlaps(newSchedule)))
            {
                _notifier.SendNotification(title: "Error",
                    message: "This schedule overlaps with an existing schedule.",
                    level: MessageLevel.Error);

                return;
            }

            Schedules.Add(newSchedule);
            NotifyAllCommands();

            if (!UserSettings.Default.ActivateSchedulesOnCreation) return;

            // we get the job key back as a way to uniquely identify
            // each schedule in the scheduling system
            var newJobKey = await _scheduler.CreateScheduleAsync(newSchedule);
            newSchedule.IsActive = true;
            newSchedule.Key = newJobKey;
        }
    }
}
