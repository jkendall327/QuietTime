using System.Threading.Tasks;
using Quartz;
using QuietTime.Core.Models;

namespace QuietTime.Core.Services.Scheduling
{
    /// <summary>
    /// Encapsulates queueing up a <see cref="Schedule"/> for later execution.
    /// </summary>
    public interface ISchedulingService
    {
        /// <summary>
        /// Gives a user's schedule to the scheduler for later execution.
        /// </summary>
        /// <param name="userSchedule">The times and volumes to be executed later.</param>
        /// <returns>A <see cref="JobKey"/> for uniquely identifying this schedule.</returns>
        Task<JobKey> CreateScheduleAsync(Schedule userSchedule);

        /// <summary>
        /// Deletes a schedule permanently.
        /// </summary>
        /// <param name="key">The unique key of the job to be deletes.</param>
        /// <returns>true if the schedule was found and deleted succesfully.</returns>
        Task<bool> DeleteScheduleAsync(JobKey key);

        /// <summary>
        /// Activates paused schedules and pauses activated schedules.
        /// </summary>
        /// <param name="key">The unique <see cref="JobKey"/> of the schedule to be paused.</param>
        Task FlipScheduleActivation(JobKey key);

        /// <summary>
        /// Pauses all schedules.
        /// </summary>
        Task PauseAll();

        /// <summary>
        /// Activates all schedules.
        /// </summary>
        Task ResumeAll();
    }
}