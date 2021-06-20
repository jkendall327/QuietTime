using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using QuietTime.Other;
using System;
using System.IO;
using System.Reflection;

namespace QuietTime.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        readonly IConfiguration _config;
        readonly ILogger _log;

        readonly MMDevice _device;

        private int _currentVolume;
        public int CurrentVolume
        {
            get { return _currentVolume; }
            set { SetProperty(ref _currentVolume, value); }
        }

        private int _maxVolume;
        public int MaxVolume
        {
            get { return _maxVolume; }
            set { SetProperty(ref _maxVolume, value); }
        }

        public string VersionInfo
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

                return $"v{version}, built {buildDate:g}";
            }
        }

        public MainWindowVM(MMDeviceEnumerator enumerator, IConfiguration config, ILogger log)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            if (enumerator is null)
            {
                throw new ArgumentNullException(nameof(enumerator));
            }

            MaxVolume = _config.GetValue<int>("InitialMaxVolume");

            // hook up the audio device
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;

            CurrentVolume = _device.AudioEndpointVolume.MasterVolumeLevelScalar.ToPercentage();
        }

        private void OnVolumeChange(AudioVolumeNotificationData data)
        {
            _log.LogInformation("Volume change");

            var volume = data.MasterVolume.ToPercentage();

            if (volume > MaxVolume)
            {
                float newLevel = (float)MaxVolume / 100;
                _device.AudioEndpointVolume.MasterVolumeLevelScalar = newLevel;
            }

            CurrentVolume = volume;
        }
    }
}
