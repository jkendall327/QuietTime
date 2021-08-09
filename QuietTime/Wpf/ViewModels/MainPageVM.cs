using System;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Core.AudioLocking;
using QuietTime.Core.Logging;
using QuietTime.Settings;

namespace QuietTime.Wpf.ViewModels
{
    internal class MainPageVM : AudioAwareBaseVM
    {
        public int NewMaxVolume
        {
            get { return _newMaxVolume; }
            set { SetProperty(ref _newMaxVolume, value); }
        }
        private int _newMaxVolume;

        public bool IsLocked
        {
            get { return _isLocked; }
            set { SetProperty(ref _isLocked, value); }
        }
        private bool _isLocked;

        public RelayCommand LockVolume { get; set; }
        public RelayCommand<VolumeAmounts> ChangeVolumeCommand { get; set; }
        public RelayCommand<MouseWheelEventArgs> MousewheelChangeVolumeCommand { get; set; }

        public MainPageVM(ILogger<MainPageVM> logger, IAudioLocker audio) : base(audio)
        {
            logger.LogInformation(EventIds.AppStartup, "App booted succesfully.");

            NewMaxVolume = UserSettings.Default.DefaultMaxVolume;

            if (UserSettings.Default.LockOnStartup)
            {
                audio.SwitchLock(NewMaxVolume);
            }

            ChangeVolumeCommand = new((val) =>
            {
                int amount = val switch
                {
                    VolumeAmounts.SmallIncrease => 5,
                    VolumeAmounts.LargeIncrease => 15,
                    VolumeAmounts.SmallDecrease => -5,
                    VolumeAmounts.LargeDecrease => -15,
                    _ => 0
                };

                ChangeNewMaxVolume(amount);
            });


            // delta is how much mousewheel was moved
            // divide by ten because it's too big otherwise
            MousewheelChangeVolumeCommand = new RelayCommand<MouseWheelEventArgs>((e) =>
            {
                ChangeNewMaxVolume(e!.Delta / 10);
            });

            LockVolume = new RelayCommand(() => audio.SwitchLock(NewMaxVolume));

            audio.LockStatusChanged += (s, e) => IsLocked = e;
        }

        private void ChangeNewMaxVolume(int amount)
        {
            NewMaxVolume += amount;
            NewMaxVolume = Math.Clamp(NewMaxVolume, 0, 100);
        }
    }
}
