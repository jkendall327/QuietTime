using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAudio.CoreAudioApi;
using QuietTime.Other;
using System;

namespace QuietTime.Services
{
    /// <summary>
    /// Encapsulates manipulation of system audio through the NAudio library.
    /// </summary>
    public class AudioService
    {
        private readonly ILogger<AudioService> _log;
        private readonly MMDevice _device;

        /// <summary>
        /// Fires when <see cref="IsLocked"/> changes. The boolean value represents whether system audio is currently locked.
        /// </summary>
        public event EventHandler<bool>? LockStatusChanged;

        /// <summary>
        /// Fires when <see cref="CurrentVolume"/> changes. The int represents the new volume as a percentage.
        /// </summary>
        public event EventHandler<int>? VolumeChanged;

        /// <summary>
        /// Fires when <see cref="MaxVolume"/> changes. The int represents the new max volume as a percentage.
        /// </summary>
        public event EventHandler<int>? MaxVolumeChanged;

        /// <summary>
        /// Creates a new <see cref="AudioService"/>.
        /// </summary>
        /// <param name="config">Program configuration.</param>
        /// <param name="log">Logging framework for this class.</param>
        /// <param name="enumerator">NAudio link that provides access to system audio.</param>
        public AudioService(IOptions<Settings> config, ILogger<AudioService> log, MMDeviceEnumerator enumerator)
        {
            _log = log;

            // get audio device
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;

            // this probably doesn't need to exist
            MaxVolume = config.Value.InitialMaxVolume;
        }

        /// <summary>
        /// The system's current volume, as a percentage.
        /// </summary>
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

            _log.LogInformation(EventIds.VolumeLimitHit, "Volume limit of {MaxVolume} hit.", MaxVolume);

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

            _log.LogInformation(EventIds.MaxVolumeChanged, "Max volume changed to {max}", MaxVolume);

            if (IsLocked)
            {
                ClampAudio();
            }

            LockStatusChanged?.Invoke(this, IsLocked);
            MaxVolumeChanged?.Invoke(this, MaxVolume);
        }
    }
}
