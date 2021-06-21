﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NAudio.CoreAudioApi;
using Quartz;
using Quartz.Impl;
using QuietTime.Models;
using QuietTime.Other;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace QuietTime.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        #region Fields

        // general services
        readonly IConfiguration _config;
        readonly ILogger _log;

        /// <summary>
        /// Represents the user's audio.
        /// </summary>
        readonly MMDevice _device;

        /// <summary>
        /// The current level of the user's volume.
        /// </summary>
        public int CurrentVolume
        {
            get { return _currentVolume; }
            set { SetProperty(ref _currentVolume, value); }
        }
        private int _currentVolume;

        /// <summary>
        /// The loudest volume currently allowed. No effect if <see cref="IsLocked"/> is false.
        /// </summary>
        public int MaxVolume
        {
            get { return _maxVolume; }
            set { SetProperty(ref _maxVolume, value); }
        }
        private int _maxVolume;

        /// <summary>
        /// What <see cref="MaxVolume"/> will be set to when <see cref="OnLock"/> is called.
        /// </summary>
        public int NewMaxVolume
        {
            get { return _newMaxVolume; }
            set { SetProperty(ref _newMaxVolume, value); }
        }
        private int _newMaxVolume;

        /// <summary>
        /// Returns the current assembly's version and build date.
        /// </summary>
        public string VersionInfo
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

                return $"v{version}, built {buildDate:d}";
            }
        }

        /// <summary>
        /// Whether the user's audio is currently locked.
        /// </summary>
        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set { SetProperty(ref _isLocked, value); }
        }


        /// <summary>
        /// Locks volume at current levels.
        /// </summary>
        public RelayCommand LockVolume
        {
            get { return _lockVolume; }
            set { SetProperty(ref _lockVolume, value); }
        }
        private RelayCommand _lockVolume;

        #endregion

        public MainWindowVM(MMDeviceEnumerator enumerator, IConfiguration config, ILogger log)
        {
            _config = config;
            _log = log;
            _lockVolume = new(OnLock);

            // hook up events
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;

            CurrentVolume = _device.AudioEndpointVolume.MasterVolumeLevelScalar.ToPercentage();
            MaxVolume = _config.GetValue<int>("InitialMaxVolume");
        }

        public async Task StartScheduler()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            var userSchedule = new Schedule(TimeOnly.Parse("10:48"), TimeOnly.Parse("15:00"), 20, 40);

            var details = ScheduleToJob(userSchedule);

            await scheduler.ScheduleJob(details.job, details.trigger);
        }

        private (IJobDetail job, ITrigger trigger) ScheduleToJob(Schedule userSchedule)
        {
            var jobdata = new JobDataMap
            {
                { "work", new Action(() => { MaxVolume = userSchedule.VolumeDuring; IsLocked = true; }) }
            };

            IJobDetail job = JobBuilder.Create<ChangeMaxVolumeJob>()
                .WithIdentity("job1", "group1")
                .SetJobData(jobdata)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(DateTimeOffset.Parse(userSchedule.Start.ToString()))
                .WithSimpleSchedule(x =>
                {
                    x.WithIntervalInHours(24);
                    x.RepeatForever();
                })
                .Build();

            return (job, trigger);
        }

        private class ChangeMaxVolumeJob : IJob
        {
            async Task IJob.Execute(IJobExecutionContext context)
            {
                var action = (Action)context.JobDetail.JobDataMap.Get("work");
                action();
            }
        }

        /// <summary>
        /// Locks or unlocks volume levels when button is clicked.
        /// </summary>
        private void OnLock()
        {
            IsLocked = !IsLocked;
            MaxVolume = IsLocked ? NewMaxVolume : 0;
        }

        /// <summary>
        /// Actually clamps the volume when volume-change event triggers.
        /// </summary>
        private void OnVolumeChange(AudioVolumeNotificationData data)
        {
            if (!IsLocked) return;

            var volume = data.MasterVolume.ToPercentage();

            if (volume > MaxVolume)
            {
                _log.LogInformation($"Volume limit of {MaxVolume} hit.");

                float newLevel = (float)MaxVolume / 100;
                _device.AudioEndpointVolume.MasterVolumeLevelScalar = newLevel;
            }

            CurrentVolume = volume;
        }
    }
}
