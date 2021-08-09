using System.Collections.Generic;
using System.Threading.Tasks;
using QuietTime.Core.Scheduling;

namespace QuietTime.Serialization
{
    /// <summary>
    /// Encapsulates serializing a user's schedules.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes a user's schedules.
        /// </summary>
        /// <returns>A collection of the user's schedules.</returns>
        IEnumerable<Schedule> DeserializeSchedules();

        /// <summary>
        /// Serializes the user's schedules.
        /// </summary>
        Task SerializeSchedulesAsync(IEnumerable<Schedule> schedules);
    }
}