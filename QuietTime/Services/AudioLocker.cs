using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using QuietTime.Core.Other;
using QuietTime.Core.Services;
using System;

namespace QuietTime.Services
{
    /*
     * This implementation goes in the WPF project because the NAudio library
     * has a hard reference to Windows and I want the Core project to
     * not have that link.
     */

    /// <summary>
    /// Encapsulates manipulation of system audio through the NAudio library.
    /// </summary>
    public class AudioLocker : IAudioLocker
    {
        private readonly ILogger<AudioLocker> _log;
        private readonly INotifier _notificationService;
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
        /// Creates a new <see cref="AudioLocker"/>.
        /// </summary>
        /// <param name="config">Program configuration.</param>
        /// <param name="log">Logging framework for this class.</param>
        /// <param name="enumerator">NAudio link that provides access to system audio.</param>
        /// <param name="notificationService">Provides notifications for this class.</param>
        public AudioLocker(ILogger<AudioLocker> log, MMDeviceEnumerator enumerator, INotifier notificationService)
        {
            _log = log;
            _notificationService = notificationService;

            // get audio device
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            // get notified when system audio volume changes
            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;
        }

        /// <summary>
        /// The system's current volume, as a percentage.
        /// </summary>
        public int CurrentVolume => _device.AudioEndpointVolume.MasterVolumeLevelScalar.ToPercentage();

        public int MaxVolume { get; private set; }
        private bool IsLocked { get; set; }

        private void OnVolumeChange(AudioVolumeNotificationData data)
        {
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

            _notificationService.SendNotification(
                "Max volume changed", 
                $"Your maximum volume has been set to {MaxVolume}.", 
                MessageLevel.Information);

            if (IsLocked)
            {
                ClampAudio();
            }

            LockStatusChanged?.Invoke(this, IsLocked);
            MaxVolumeChanged?.Invoke(this, MaxVolume);
        }
    }
}
