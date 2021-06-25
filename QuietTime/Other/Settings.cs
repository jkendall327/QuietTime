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
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.