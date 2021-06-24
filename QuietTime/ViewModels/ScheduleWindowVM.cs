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
    public class ScheduleWindowVM : ObservableObject
    {
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
        private Schedule _schedule = new(TimeOnly.MinValue, TimeOnly.MinValue, 0, 0);

        /// <summary>
        /// Creates an initially-inactive schedule.
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

        private readonly SchedulerService _scheduler;
        private readonly Settings _config;
        private readonly ILogger<ScheduleWindowVM> _logger;
        private readonly NotificationService _notifications;

        /// <summary>
        /// Creates a new <see cref="ScheduleWindowVM"/>.
        /// </summary>
        /// <param name="scheduler">Used to pass schedules created here into the scheduling back-end.</param>
        /// <param name="config">Application configuration.</param>
        /// <param name="logger">Logging framework for this class.</param>
        /// <param name="notifications">Notification service for this class.</param>
        public ScheduleWindowVM(SchedulerService scheduler, IOptions<Settings> config, ILogger<ScheduleWindowVM> logger, NotificationService notifications)
        {
            _scheduler = scheduler;
            _config = config.Value;
            _logger = logger;
            _notifications = notifications;

            AddSchedule = new AsyncRelayCommand(AddScheduleAsync);
            DeleteSelected = new AsyncRelayCommand(RemoveScheduleAsync);
            DeactivateAll = new AsyncRelayCommand(DeactivateAllAsync);
            FlipActivation = new AsyncRelayCommand(FlipActivationAsync);
            ActivateAll = new AsyncRelayCommand(ActivateAllAsync);
            Serialize = new AsyncRelayCommand(SerializeSchedules);

            foreach (var schedule in DeserializeSchedules())
            {
                Schedules.Add(schedule);
            }
        }

        private IEnumerable<Schedule> DeserializeSchedules()
        {
            var filepath = _config.SerializedDataFilename;

            try
            {
                var result = JsonSerializer.Deserialize<ObservableCollection<Schedule>>(File.ReadAllText(filepath));

                if (result is null)
                {
                    _logger.LogError(EventIds.DeserializationError, "File {file} was null on deserialization.", filepath);
                    return Enumerable.Empty<Schedule>();
                }

                _logger.LogInformation(EventIds.DeserializationSuccess, "File {file} deserialized succesfully.", filepath);
                return result;
            }
            catch (FileNotFoundException ex)
            {
                 _logger.LogError(EventIds.DeserializationError, ex, "File {filepath} loaded from app config file was not found.", filepath);

                return Enumerable.Empty<Schedule>();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(EventIds.DeserializationError, ex, "Argument was null when deserializing {file}.", filepath);
                return Enumerable.Empty<Schedule>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(EventIds.DeserializationError, ex, "Improper JSON detected while deserializing {file}.", filepath);
                return Enumerable.Empty<Schedule>();
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.DeserializationError, ex, "Exception while loading or deserializing {file}.", filepath);
                return Enumerable.Empty<Schedule>();
            }
        }

        private async Task SerializeSchedules()
        {
            var filepath = _config.SerializedDataFilename;

            try
            {
                using var fs = new FileStream(filepath, FileMode.OpenOrCreate);

                await JsonSerializer.SerializeAsync<ObservableCollection<Schedule>>(fs, Schedules, new JsonSerializerOptions()
                {
                    WriteIndented = true
                });

                _logger.LogInformation(EventIds.SerializationSuccess, "File {file} serialized succesfully.", filepath);
                _notifications.SendNotification("Success", "Your schedules have been succesfully saved.", NotificationService.MessageLevel.Information);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(EventIds.SerializationError, ex, "File {file} loaded from app config file was not found}.", filepath);
                _notifications.SendNotification("Error", "There was an issue saving your schedules. You may have to restart QuietTime and try again.", NotificationService.MessageLevel.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.SerializationError, ex, "Exception when deserializing {file}.", filepath);
                _notifications.SendNotification("Error", "There was an issue saving your schedules. You may have to restart QuietTime and try again.", NotificationService.MessageLevel.Information);
            }
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

            newSchedule.Key = await _scheduler.CreateScheduleAsync(newSchedule);
        }
    }
}
