namespace QuietTime.Core.Notifications
{
    /// <summary>
    /// Level of importance of notifications sent to user.
    /// </summary>
    public enum MessageLevel
    {
        /// <summary>
        /// Message has unclassified importance.
        /// </summary>
        None,

        /// <summary>
        /// Message indicates no issues with the system.
        /// </summary>
        Information,

        /// <summary>
        /// Message indicates a failure that the system can work through.
        /// </summary>
        Warning,

        /// <summary>
        /// Message indicates a catastrophic failure the system can not recover from.
        /// </summary>
        Error
    }
}
