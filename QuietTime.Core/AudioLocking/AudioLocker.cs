using System;
using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using QuietTime.Core.Logging;
using QuietTime.Core.Notifications;

namespace QuietTime.Core.AudioLocking
{
    /// <summary>
    /// <inheritdoc cref="IAudioLocker"/>
    /// </summary>
    internal class AudioLocker : IAudioLocker
    {
        private readonly ILogger<AudioLocker> _log;
        private readonly INotifier _notificationService;
        private readonly MMDevice _device;

        public event EventHandler<bool>? LockStatusChanged;
        public event EventHandler<int>? VolumeChanged;
        public event EventHandler<int>? MaxVolumeChanged;

        public AudioLocker(ILogger<AudioLocker> log, MMDeviceEnumerator enumerator, INotifier notificationService)
        {
            _log = log;
            _notificationService = notificationService;

            // get audio device
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            // get notified when system audio volume changes
            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;
        }

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
