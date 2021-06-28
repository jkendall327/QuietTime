using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;
using QuietTime.Other;
using QuietTime.Services;
using System;
using System.Windows;

namespace QuietTime
{
    public class ScheduleJobFactory : IJobFactory
    {
        private readonly AudioService _audioService;
        private readonly ILogger<ChangeMaxVolumeJob> _logger;

        public ScheduleJobFactory(AudioService audioService, ILogger<ChangeMaxVolumeJob> logger)
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
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
