using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietTime.Core.Services
{
    public interface IAudioLocker
    {
        /// <summary>
        /// Fires when <see cref="IsLocked"/> changes. The boolean value represents whether system audio is currently locked.
        /// </summary>
        event EventHandler<bool>? LockStatusChanged;

        /// <summary>
        /// Fires when <see cref="CurrentVolume"/> changes. The int represents the new volume as a percentage.
        /// </summary>
        event EventHandler<int>? VolumeChanged;

        /// <summary>
        /// Fires when <see cref="MaxVolume"/> changes. The int represents the new max volume as a percentage.
        /// </summary>
        event EventHandler<int>? MaxVolumeChanged;

        void SwitchLock(int newMaxVolume);

        int CurrentVolume { get; }

        int MaxVolume { get; }
    }
}
