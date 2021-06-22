using Microsoft.Extensions.Logging;
using Quartz;
using QuietTime.Models;
using QuietTime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietTime.Other
{
    public class SchedulerService
    {
        private readonly IScheduler _scheduler;
        private readonly ILogger _logger;
        private readonly AudioService _audio;

        public SchedulerService(IScheduler scheduler, ILogger logger, AudioService audio)
        {
            _scheduler = scheduler;
            _logger = logger;
            _audio = audio;
        }

        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        public async Task StartAsync() 
        {
            if (_scheduler.IsStarted) return;

            await _scheduler.Start(); 
        }

        /// <summary>
        /// Place the scheduler on standby. Restart with the <see cref="StartAsync"/> method.
        /// </summary>
        public async Task StopAsync()
        {
            if (!_scheduler.InStandbyMode) return;

            await _scheduler.Standby();
        }

        private string NewGuid => Guid.NewGuid().ToString();

        /// <summary>
        /// Gives a user's schedule to the scheduler for later execution.
        /// </summary>
        /// <param name="userSchedule">The times and volumes to be executed later.</param>
        /// <returns>A <see cref="JobKey"/> for uniquely identifying this schedule.</returns>
        public async Task<JobKey> CreateScheduleAsync(Schedule userSchedule)
        {
            // create job data that's called later to actually change the volume
            var OnStart = new Action(() => _audio.SwitchLock(userSchedule.VolumeDuring));
            var OnEnd = new Action(() => _audio.SwitchLock(userSchedule.VolumeAfter));
            var first = new JobDataMap {{ "work", OnStart }, { "logger", _logger } };
            var second = new JobDataMap {{ "work", OnEnd }, { "logger", _logger } };

            var groupGUID = NewGuid;
            var jobIdentity = NewGuid;

            // create job
            IJobDetail job = JobBuilder.Create<ChangeMaxVolumeJob>()
                .WithIdentity(jobIdentity, groupGUID)
                .Build();

            // each trigger has its own jobdatamap, letting us schedule multiple effects for the same job
            ITrigger startTrigger = MakeTrigger(NewGuid, groupGUID, first, userSchedule.Start.ToString());
            ITrigger endTrigger = MakeTrigger(NewGuid, groupGUID, second, userSchedule.End.ToString());

            _logger.LogInformation(
                new EventId(1, "Job creation"), 
                "Job scheduled: GUID {job}, " +
                "with group ID {group}" +
                "starting {start} " +
                "and finishing {end}.",
                jobIdentity, groupGUID, userSchedule.Start, userSchedule.End);

            // actually schedule the job
            await _scheduler.ScheduleJob(job, new List<ITrigger>() { startTrigger, endTrigger }, replace: true);

            return job.Key;
        }

        /// <summary>
        /// Deletes a schedule permanently.
        /// </summary>
        /// <param name="key">The unique key of the job to be deletes.</param>
        /// <returns>true if the schedule was found and deleted succesfully.</returns>
        public async Task<bool> DeleteScheduleAsync(JobKey key)
        {
            _logger.LogInformation(new EventId(3, "Job deleted"), "Job {key} deleted.", key);
            return await _scheduler.DeleteJob(key);
        }

        public async Task PauseSchedule(JobKey key)
        {
            await _scheduler.PauseJob(key);
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
                anyPaused = await _scheduler.GetTriggerState(triggerKey) == TriggerState.Paused;
            }

            return anyPaused;
        }

        public async Task PauseAll() => await _scheduler.PauseAll();
        public async Task ResumeAll() => await _scheduler.ResumeAll();

        private static ITrigger MakeTrigger(string identity, string guid, JobDataMap data, string dateTimeOffset)
        {
            return TriggerBuilder.Create()
                .WithIdentity(identity, guid)
                .UsingJobData(data)
                .StartAt(DateTimeOffset.Parse(dateTimeOffset))
                .WithSimpleSchedule(x =>
                {
                    x.WithIntervalInHours(24);
                    x.RepeatForever();
                })
                .Build();
        }

        private class ChangeMaxVolumeJob : IJob
        {
            async Task IJob.Execute(IJobExecutionContext context)
            {
                await Task.Delay(0);

                // retrieve the action we set on job creation
                var action = (Action)context.Trigger.JobDataMap.Get("work");
                action?.Invoke();

                var logger = (ILogger)context.Trigger.JobDataMap.Get("logger");
                logger?.LogInformation(
                    new EventId(2, "Job performed"),
                    "Job {jobKey} ran for trigger {triggerKey}.",
                    context.JobDetail.Key, context.Trigger.Key);
            }
        }
    }
}
