using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using QuietTime.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietTime.Services
{
    public class AudioService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AudioService> _log;
        private readonly MMDevice _device;

        public event EventHandler<bool>? LockStatusChanged;
        public event EventHandler<int>? VolumeChanged;

        public AudioService(IConfiguration config, ILogger<AudioService> log, MMDeviceEnumerator enumerator)
        {
            _config = config;
            _log = log;

            // get audio device
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;

            // this probably doesn't need to exist
            MaxVolume = _config.GetValue<int>("InitialMaxVolume");
        }

        public int CurrentVolume => _device.AudioEndpointVolume.MasterVolumeLevelScalar.ToPercentage();

        private int MaxVolume { get; set; }
        private bool IsLocked { get; set; }

        private void OnVolumeChange(AudioVolumeNotificationData data)
        {
            var volume = data.MasterVolume.ToPercentage();

            ClampAudio();

            VolumeChanged?.Invoke(this, CurrentVolume);
        }

        private void ClampAudio()
        {
            if (CurrentVolume <= MaxVolume || !IsLocked) return;
            
            _log.LogInformation(new EventId(5, "Volume limit hit"), "Volume limit of {MaxVolume} hit.", MaxVolume);

            float newLevel = (float)MaxVolume / 100;
            _device.AudioEndpointVolume.MasterVolumeLevelScalar = newLevel;
        }

        /// <summary>
        /// Locks the maximum allowed volume.
        /// </summary>
        public void SwitchLock(int newMaxVolume)
        {
            IsLocked = !IsLocked;
            MaxVolume = Math.Clamp(newMaxVolume, 0, 100);

            _log.LogInformation(new EventId(4, "Max volume changed"), "Max volume changed to {max}", MaxVolume);

            if (IsLocked)
            {
                ClampAudio();
            }

            LockStatusChanged?.Invoke(this, IsLocked);
        }
    }
}
