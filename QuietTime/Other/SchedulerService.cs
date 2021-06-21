using Microsoft.Extensions.Logging;
using Quartz;
using QuietTime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietTime.Other
{
    public class SchedulerService
    {
        private IScheduler _scheduler { get; }
        public ILogger _logger { get; }

        public SchedulerService(IScheduler scheduler, ILogger logger)
        {
            _scheduler = scheduler;
            _logger = logger;
        }

        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        public async Task StartAsync() => await _scheduler.Start();

        /// <summary>
        /// Place the scheduler on standby. Restart with the <see cref="StartAsync"/> method.
        /// </summary>
        public async Task StopAsync() => await _scheduler.Standby();

        /// <summary>
        /// Gives a user's schedule to the scheduler for later execution.
        /// </summary>
        /// <param name="userSchedule">The times and volumes to be executed later.</param>
        /// <param name="onStart">The action to be performed at <see cref="Schedule.Start"/>.</param>
        /// <param name="onEnd">he action to be performed at <see cref="Schedule.End"/>.</param>
        public async Task CreateScheduleAsync(Schedule userSchedule, Action onStart, Action onEnd)
        {
            var first = new JobDataMap {{ "work", onStart }};
            var second = new JobDataMap {{ "work", onEnd }};

            var groupGUID = Guid.NewGuid().ToString();

            IJobDetail job = JobBuilder.Create<ChangeMaxVolumeJob>()
                .WithIdentity("job1", groupGUID)
                .Build();

            ITrigger startTrigger = MakeTrigger(groupGUID, first, userSchedule.Start.ToString());
            ITrigger endTrigger = MakeTrigger(groupGUID, second, userSchedule.End.ToString());

            _logger.LogInformation(
                $"Job scheduled: GUID {groupGUID}, " +
                $"starting {userSchedule.Start} " +
                $"and finishing {userSchedule.End}");

            await _scheduler.ScheduleJob(job, new List<ITrigger>() { startTrigger, endTrigger }, replace: true);
        }

        private static ITrigger MakeTrigger(string guid, JobDataMap data, string dateTimeOffset)
        {
            return TriggerBuilder.Create()
                .WithIdentity("trigger2", guid)
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

                /*
                 * each trigger has its own jobdatamap and in turn its own Action
                 * hence letting us schedule different effects for the same job
                 * via the two triggers
                 */
                var action = (Action)context.Trigger.JobDataMap.Get("work");
                action();
            }
        }
    }
}
