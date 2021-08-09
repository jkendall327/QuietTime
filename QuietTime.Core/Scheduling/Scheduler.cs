using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using QuietTime.Core.AudioLocking;
using QuietTime.Core.Logging;

namespace QuietTime.Core.Scheduling
{
    /// <summary>
    /// <inheritdoc cref="ISchedulingService"/>
    /// </summary>
    public class Scheduler : ISchedulingService
    {
        private readonly IScheduler _scheduler;
        private readonly ILogger<Scheduler> _logger;
        private readonly IAudioLocker _audio;

        public Scheduler(IScheduler scheduler, ILogger<Scheduler> logger, IAudioLocker audio)
        {
            _scheduler = scheduler;
            _logger = logger;
            _audio = audio;
        }

        private static string NewGuid => Guid.NewGuid().ToString();

        public async Task<JobKey> CreateScheduleAsync(Schedule userSchedule)
        {
            // groupGUID is given to the job and both triggers, jobIdentity is just for the job
            var groupGUID = NewGuid;
            var jobIdentity = NewGuid;

            // create job
            IJobDetail job = JobBuilder.Create<ChangeMaxVolumeJob>()
                .WithIdentity(jobIdentity, groupGUID)
                .Build();

            // multiple triggers let us schedule separate effects for schedule start and end
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

        private static ITrigger MakeTrigger(string guid, string dateTimeOffset, int volume) =>
            TriggerBuilder.Create()
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

        public async Task<bool> DeleteScheduleAsync(JobKey key)
        {
            _logger.LogInformation(EventIds.JobDeleted, "Job {key} deleted.", key);
            _audio.SwitchLock(100);

            return await _scheduler.DeleteJob(key);
        }

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
                // todo change this, what if one's true and the next is false?
                anyPaused = await _scheduler.GetTriggerState(triggerKey) == TriggerState.Paused;
            }

            return anyPaused;
        }

        public async Task PauseAll() => await _scheduler.PauseAll();
        public async Task ResumeAll() => await _scheduler.ResumeAll();
    }
}
