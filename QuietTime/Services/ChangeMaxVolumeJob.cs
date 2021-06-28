using Microsoft.Extensions.Logging;
using Quartz;
using QuietTime.Services;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace QuietTime.Other
{
    public class ChangeMaxVolumeJob : IJob
    {
        // this just provides strongly-typed access to the key-value pair
        public const string VolumeKey = "volume";

        private readonly AudioService _audioService;
        private readonly ILogger<ChangeMaxVolumeJob> _logger;

        private readonly Dispatcher _dispatcher;

        public ChangeMaxVolumeJob(AudioService audioService, ILogger<ChangeMaxVolumeJob> logger, Dispatcher dispatcher)
        {
            _audioService = audioService;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            var volume = (int)context.Trigger.JobDataMap.Get(VolumeKey);

            // we have to use the dispatcher because
            // the audio service sends a UI notification
            // todo: put dispatcher call in notification class
            // to avoid this trouble?
            _dispatcher.Invoke(() =>
            {
                _audioService.SwitchLock(volume);
            });

            _logger.LogInformation(EventIds.JobPerformed,
                "Job {jobKey} ran for trigger {triggerKey}. Trigger description: {description}",
                context.JobDetail.Key, context.Trigger.Key, context.Trigger.Description);

            return Task.CompletedTask;
        }
    }
}
