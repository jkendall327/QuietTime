using Microsoft.Extensions.Logging;

namespace QuietTime.Core.Logging
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
        /// Used when deserializing a user's schedules.
        /// </summary>
        public static readonly EventId Deserialization = new(8, "Deserialization");

        /// <summary>
        /// Used when serializing a user's schedules.
        /// </summary>
        public static readonly EventId Serialization = new(10, "Serialization");

        /// <summary>
        /// Used when user makes QuietTime load on sign-in.
        /// </summary>
        public static readonly EventId AutomaticStartupAdded = new(12, "Auto-start added");

        /// <summary>
        /// Used when user stops QuietTime loading on sign-in.
        /// </summary>
        public static readonly EventId AutomaticStartupRemoved = new(13, "Auto-start removed");

        /// <summary>
        /// Used when stopping QuietTime from starting on sign-in fails..
        /// </summary>
        public static readonly EventId AutomaticStartupFailure = new(14, "Start-up removal failure");
    }
}
