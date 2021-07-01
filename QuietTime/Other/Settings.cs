#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace QuietTime
{

    
    /// <summary>
    /// Provides strongly-typed program settings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Max volume when opening program.
        /// </summary>
        public int InitialMaxVolume { get; set; }

        /// <summary>
        /// Filename for user's serialized schedules.
        /// </summary>
        public string SerializedDataFilename { get; set; }

        /// <summary>
        /// Whether toast notifications should be enabled.
        /// </summary>
        public bool NotificationsEnabled { get; set; }

        /// <summary>
        /// Whether the app will minimize to the system tray when the window is closed.
        /// </summary>
        public bool MinimizeOnClose { get; set; }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.