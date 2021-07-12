using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using QuietTime.Core.Other;

namespace QuietTime.Core.Services.Scheduling
{
    public class ChangeMaxVolumeJob : IJob
    {
        // this just provides strongly-typed access to the key-value pair
        public const string VolumeKey = "volume";

        private readonly IAudioLocker _audioService;
        private readonly ILogger<ChangeMaxVolumeJob> _logger;

        public ChangeMaxVolumeJob(IAudioLocker audioService, ILogger<ChangeMaxVolumeJob> logger)
        {
            _audioService = audioService;
            _logger = logger;
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            var volume = (int)context.Trigger.JobDataMap.Get(VolumeKey);

            _audioService.SwitchLock(volume);

            _logger.LogInformation(EventIds.JobPerformed,
                "Job {jobKey} ran for trigger {triggerKey}. Trigger description: {description}",
                context.JobDetail.Key, context.Trigger.Key, context.Trigger.Description);

            return Task.CompletedTask;
        }
    }
}
