using Microsoft.Extensions.Logging;
using Quartz;
using QuietTime.Services;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace QuietTime.Other
{
    public class ChangeMaxVolumeJob : IJob
    {
        // these just provide strongly-typed access to key-value pairs
        public const string AudioServiceKey = "audioService";
        public const string VolumeKey = "volume";
        public const string LoggerKey = "logger";
        public const string NotificationServiceKey = "notifications";
        public const string DispatcherKey = "dispatcher";

        private readonly NotificationService _notificationService;
        private readonly AudioService _audioService;
        private readonly ILogger<ChangeMaxVolumeJob> _logger;

        private readonly Dispatcher _dispatcher;

        public ChangeMaxVolumeJob(NotificationService notificationService, AudioService audioService, ILogger<ChangeMaxVolumeJob> logger, Dispatcher dispatcher)
        {
            _notificationService = notificationService;
            _audioService = audioService;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        async Task IJob.Execute(IJobExecutionContext context)
        {
            // TODO: figure out why this is needed/not needed!
            await Task.Delay(1);

            var volume = (int)context.Trigger.JobDataMap.Get(VolumeKey);

            _audioService.SwitchLock(volume);

            _notificationService.SendNotification
                ("Volume changed",
                $"Your maximum volume has been set to {volume}.",
                NotificationService.MessageLevel.Information);

            // have to this because quartz.net fires jobs from another thread
            // and wpf monopolises access to the UI thread -- I think, anyway 

            _dispatcher.Invoke(() =>
            {
                _notificationService?.SendNotification
                    ("Volume changed",
                    $"Your maximum volume has been set to {volume}.",
                    NotificationService.MessageLevel.Information);
            });

            _logger.LogInformation(EventIds.JobPerformed,
                "Job {jobKey} ran for trigger {triggerKey}. Trigger description: {description}",
                context.JobDetail.Key, context.Trigger.Key, context.Trigger.Description);
        }
    }
}
