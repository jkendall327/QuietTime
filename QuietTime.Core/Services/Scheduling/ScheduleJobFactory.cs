using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;
using System;

namespace QuietTime.Core.Services.Scheduling
{
    /// <summary>
    /// Creates a new <see cref="ChangeMaxVolumeJob"/> for the scheduler.
    /// </summary>
    public class ScheduleJobFactory : IJobFactory
    {
        private readonly IAudioLocker _audioService;
        private readonly ILogger<ChangeMaxVolumeJob> _logger;

        public ScheduleJobFactory(IAudioLocker audioService, ILogger<ChangeMaxVolumeJob> logger)
        {
            _audioService = audioService;
            _logger = logger;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return new ChangeMaxVolumeJob(_audioService, _logger);
        }

        public void ReturnJob(IJob job)
        {
            // this probably isn't necessary
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
