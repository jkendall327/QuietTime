using Microsoft.Extensions.Logging;
using Quartz;
using QuietTime.Models;
using QuietTime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuietTime.Other
{
    /// <summary>
    /// Encapsulates queueing up a <see cref="Schedule"/> for later execution. Wrapper to Quartz.NET.
    /// </summary>
    public class SchedulerService
    {
        private readonly IScheduler _scheduler;
        private readonly ILogger<SchedulerService> _logger;
        private readonly AudioService _audio;

        /// <summary>
        /// Creates a new <see cref="SchedulerService"/>.
        /// </summary>
        /// <param name="scheduler">The core link to Quartz.NET.</param>
        /// <param name="logger">Logging service for this class.</param>
        /// <param name="audio">Used to actually clamp system volume when jobs fire.</param>
        public SchedulerService(IScheduler scheduler, ILogger<SchedulerService> logger, AudioService audio)
        {
            _scheduler = scheduler;
            _logger = logger;
            _audio = audio;
        }

        private string NewGuid => Guid.NewGuid().ToString();

        /// <summary>
        /// Gives a user's schedule to the scheduler for later execution.
        /// </summary>
        /// <param name="userSchedule">The times and volumes to be executed later.</param>
        /// <returns>A <see cref="JobKey"/> for uniquely identifying this schedule.</returns>
        public async Task<JobKey> CreateScheduleAsync(Schedule userSchedule)
        {
            // groupGUID is given to the job and both triggers, jobIdentity is just for the job
            var groupGUID = NewGuid;
            var jobIdentity = NewGuid;

            // create job
            IJobDetail job = JobBuilder.Create<ChangeMaxVolumeJob>()
                .WithIdentity(jobIdentity, groupGUID)
                .Build();

            // each trigger has its own jobdatamap, letting us schedule multiple effects for the same job
            ITrigger startTrigger = MakeTrigger(groupGUID, userSchedule.Start.ToString(), userSchedule.VolumeDuring);
            ITrigger endTrigger = MakeTrigger(groupGUID, userSchedule.End.ToString(), userSchedule.VolumeAfter);

            _logger.LogInformation(
                EventIds.JobCreated,
                "Job scheduled: GUID {job}, " +
                "with group ID {group}" +
                "starting {start} " +
                "and finishing {end}.",
                jobIdentity, groupGUID, userSchedule.Start, userSchedule.End);

            // actually schedule the job
            await _scheduler.ScheduleJob(job, new List<ITrigger>() { startTrigger, endTrigger }, replace: true);

            return job.Key;
        }

        private ITrigger MakeTrigger(string guid, string dateTimeOffset, int volume)
        {
            return TriggerBuilder.Create()
                .WithIdentity(NewGuid, guid)
                // put the volume into the trigger so we can get it when the job fires
                .UsingJobData(new JobDataMap() { { ChangeMaxVolumeJob.VolumeKey, volume } })
                .StartAt(DateTimeOffset.Parse(dateTimeOffset))
                .WithSimpleSchedule(x =>
                {
                    x.WithIntervalInHours(24);
                    x.RepeatForever();
                })
                .WithDescription($"Set max volume to {volume}.")
                .Build();
        }

        /// <summary>
        /// Deletes a schedule permanently.
        /// </summary>
        /// <param name="key">The unique key of the job to be deletes.</param>
        /// <returns>true if the schedule was found and deleted succesfully.</returns>
        public async Task<bool> DeleteScheduleAsync(JobKey key)
        {
            _logger.LogInformation(EventIds.JobDeleted, "Job {key} deleted.", key);
            _audio.SwitchLock(100);

            return await _scheduler.DeleteJob(key);
        }

        /// <summary>
        /// Activates paused schedules and pauses activated schedules.
        /// </summary>
        /// <param name="key">The unique <see cref="JobKey"/> of the schedule to be paused.</param>
        public async Task FlipScheduleActivation(JobKey key)
        {
            bool anyPaused = await JobHasAnyPausedTriggers(key);

            if (anyPaused)
            {
                await _scheduler.ResumeJob(key);
                return;
            }

            await _scheduler.PauseJob(key);
        }

        private async Task<bool> JobHasAnyPausedTriggers(JobKey key)
        {
            var triggers = await _scheduler.GetTriggersOfJob(key);
            var keys = triggers.Select(x => x.Key).ToList();

            bool anyPaused = false;

            foreach (var triggerKey in keys)
            {
                anyPaused = await _scheduler.GetTriggerState(triggerKey) == TriggerState.Paused;
            }

            return anyPaused;
        }

        /// <summary>
        /// Pauses all schedules.
        /// </summary>
        public async Task PauseAll() => await _scheduler.PauseAll();

        /// <summary>
        /// Activates all schedules.
        /// </summary>
        public async Task ResumeAll() => await _scheduler.ResumeAll();
    }
}
