using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuietTime.Models;
using QuietTime.Other;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuietTime.Services
{
    /// <summary>
    /// Encapsulates serializing a user's schedules.
    /// </summary>
    public class SerializerService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SerializerService> _logger;
        private readonly NotificationService _notifications;

        /// <summary>
        /// Creates a new <see cref="SerializerService"/>.
        /// </summary>
        /// <param name="config">Program configuration for this class.</param>
        /// <param name="logger">Logging framework for this class.</param>
        /// <param name="notifications">Notification service for this class.</param>
        public SerializerService(IConfiguration config, ILogger<SerializerService> logger, NotificationService notifications)
        {
            _config = config;
            _logger = logger;
            _notifications = notifications;
        }

        private string filepath => _config.GetValue<string>("SerializedDataFilename");

        internal IEnumerable<Schedule> DeserializeSchedules()
        {
            if (!File.Exists(filepath))
            {
                _logger.LogError(EventIds.DeserializationError, "Schedules file {filepath} was not found.", filepath);
                return Enumerable.Empty<Schedule>();
            }

            try
            {
                string json = File.ReadAllText(filepath);
                var result = JsonSerializer.Deserialize<IEnumerable<ScheduleDTO>>(json);

                if (result is null)
                {
                    _logger.LogError(EventIds.DeserializationError, "File {file} was null on deserialization.", filepath);
                    return Enumerable.Empty<Schedule>();
                }

                _logger.LogInformation(EventIds.DeserializationSuccess, "File {file} deserialized successfully.", filepath);

                return result.Select(r => r.ToSchedule());
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(EventIds.DeserializationError, ex, "File {filepath} was not found.", filepath);
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

        internal async Task SerializeSchedulesAsync(ObservableCollection<Schedule> schedules)
        {
            var dtos = schedules.Select(s => new ScheduleDTO(s));

            try
            {
                using var fs = new FileStream(filepath, FileMode.OpenOrCreate);

                await JsonSerializer.SerializeAsync<IEnumerable<ScheduleDTO>>(fs, dtos, new JsonSerializerOptions()
                {
                    WriteIndented = true
                });

                _logger.LogInformation(EventIds.SerializationSuccess, "File {file} serialized succesfully.", filepath);
                _notifications.SendNotification("Success", "Your schedules have been successfully saved.", NotificationService.MessageLevel.Information);
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
    }
}
