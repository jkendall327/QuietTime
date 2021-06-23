using Microsoft.Extensions.Logging;
using QuietTime.Models;
using QuietTime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietTime.Other
{
    /// <summary>
    /// Provides strongly-typed access to <see cref="EventId"/> instances for logging.
    /// </summary>
    public static class EventIds
    {
        /// <summary>
        /// Used when the application first starts.
        /// </summary>
        public static readonly EventId AppStartup = new(0, "Startup");

        /// <summary>
        /// Used when a <see cref="Schedule"/> is added to the scheduler.
        /// </summary>
        public static readonly EventId JobCreated = new(1, "Job creation");

        /// <summary>
        /// Used when a <see cref="Schedule"/> is performed by the scheduler.
        /// </summary>
        public static readonly EventId JobPerformed = new(2, "Job performed");

        /// <summary>
        /// Used when a <see cref="Schedule"/> is deleted from the scheduler.
        /// </summary>
        public static readonly EventId JobDeleted = new(3, "Job deleted");

        /// <summary>
        /// Used when the max volume limit is changed in the <see cref="AudioService"/>.
        /// </summary>
        public static readonly EventId MaxVolumeChanged = new(4, "Max volume changed");

        /// <summary>
        /// Used when the volume limit is hit in the <see cref="AudioService"/>.
        /// </summary>
        public static readonly EventId VolumeLimitHit = new(5, "Volume limit hit");

        /// <summary>
        /// Used when the app exits.
        /// </summary>
        public static readonly EventId AppClosing = new(6, "App closed");

        /// <summary>
        /// Used when the app is closed by the user but returns to the system tray instead.
        /// </summary>
        public static readonly EventId AppClosingCancelled = new(7, "App close cancelled");

        /// <summary>
        /// Used when deserializing a user's schedules fails.
        /// </summary>
        public static readonly EventId DeserializationError = new(8, "Deserialization error");

        /// <summary>
        /// Used when deserializing a user's schedules succeeds.
        /// </summary>
        public static readonly EventId DeserializationSuccess = new(9, "Deserialization success");

        /// <summary>
        /// Used when serializing a user's schedules fails.
        /// </summary>
        public static readonly EventId SerializationError = new(10, "Deserialization error");

        /// <summary>
        /// Used when serializing a user's schedules succeeds.
        /// </summary>
        public static readonly EventId SerializationSuccess = new(11, "Deserialization success");
    }
}
