using System;

namespace QuietTime.Core.AudioLocking
{
    /// <summary>
    /// Encapsulates manipulation of system audio through the NAudio library.
    /// </summary>
    public interface IAudioLocker
    {
        /// <summary>
        /// Fires when <see cref="IsLocked"/> changes. The boolean value represents whether system audio is currently locked.
        /// </summary>
        event EventHandler<bool> LockStatusChanged;

        /// <summary>
        /// Fires when <see cref="CurrentVolume"/> changes. The int represents the new volume as a percentage.
        /// </summary>
        event EventHandler<int> VolumeChanged;

        /// <summary>
        /// Fires when <see cref="MaxVolume"/> changes. The int represents the new max volume as a percentage.
        /// </summary>
        event EventHandler<int> MaxVolumeChanged;

        /// <summary>
        /// Locks or unlocks the system audio.
        /// </summary>
        /// <param name="newMaxVolume">What the system audio should be locked to.</param>
        void SwitchLock(int newMaxVolume);

        /// <summary>
        /// The system's current volume, as a percentage.
        /// </summary>
        int CurrentVolume { get; }

        /// <summary>
        /// Max currently-allowed volume.
        /// </summary>
        int MaxVolume { get; }
    }
}
