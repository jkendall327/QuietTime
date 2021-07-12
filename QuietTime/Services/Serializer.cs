using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuietTime.Core.Models;
using QuietTime.Core.Other;
using QuietTime.Core.Services;
using QuietTime.Models;

namespace QuietTime.Services
{
    /// <summary>
    /// <inheritdoc cref="ISerializer"/>
    /// </summary>
    internal class Serializer : ISerializer
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Serializer> _logger;
        private readonly INotifier _notifications;

        public Serializer(IConfiguration config, ILogger<Serializer> logger, INotifier notifications)
        {
            _config = config;
            _logger = logger;
            _notifications = notifications;
        }

        private string filepath => _config.GetValue<string>("SerializedDataFilename");

        public IEnumerable<Schedule> DeserializeSchedules()
        {
            if (!File.Exists(filepath))
            {
                _logger.LogError(EventIds.Deserialization, "Schedules file {filepath} was not found.", filepath);
                return Enumerable.Empty<Schedule>();
            }

            try
            {
                // convert to ScheduleDTO and back because Schedule has fields that are awkward to serialize

                string json = File.ReadAllText(filepath);
                var result = JsonSerializer.Deserialize<IEnumerable<ScheduleDTO>>(json);

                if (result is null)
                {
                    _logger.LogError(EventIds.Deserialization, "File {file} was null on deserialization.", filepath);
                    return Enumerable.Empty<Schedule>();
                }

                _logger.LogInformation(EventIds.Deserialization, "File {file} deserialized successfully.", filepath);

                return result.Select(r => r.ToSchedule());
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Deserialization, ex, "Exception while loading or deserializing {file}.", filepath);
                return Enumerable.Empty<Schedule>();
            }
        }

        public async Task SerializeSchedulesAsync(IEnumerable<Schedule> schedules)
        {
            var dtos = schedules.Select(s => new ScheduleDTO(s));

            try
            {
                using var fs = new FileStream(filepath, FileMode.OpenOrCreate);

                await JsonSerializer.SerializeAsync<IEnumerable<ScheduleDTO>>(fs, dtos, new JsonSerializerOptions()
                {
                    WriteIndented = true
                });

                _logger.LogInformation(EventIds.Serialization, "File {file} serialized succesfully.", filepath);
                _notifications.SendNotification("Success", "Your schedules have been successfully saved.", MessageLevel.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Serialization, ex, "Exception when deserializing {file}.", filepath);
                _notifications.SendNotification("Error", "There was an issue saving your schedules. You may have to restart QuietTime and try again.", MessageLevel.Information);
            }
        }
    }
}
