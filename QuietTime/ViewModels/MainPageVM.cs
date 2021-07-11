using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Core.Other;
using QuietTime.Core.Services;
using QuietTime.Other;
using QuietTime.Services;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace QuietTime.ViewModels
{
    public class MainPageVM : AudioAwareBaseVM
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
        public RelayCommand IncreaseVolume { get; set; }
        public RelayCommand DecreaseVolume { get; set; }

        public RelayCommand<MouseWheelEventArgs> ChangeVolumeCommand { get; set; }

        public MainPageVM(ILogger<MainPageVM> logger, IAudioLocker audio) : base(audio)
        {
            logger.LogInformation(EventIds.AppStartup, "App booted succesfully.");

            NewMaxVolume = UserSettings.Default.DefaultMaxVolume;

            if (UserSettings.Default.LockOnStartup)
            {
                audio.SwitchLock(NewMaxVolume);
            }

            // todo: parameterize this command
            IncreaseVolume = new RelayCommand(() => ChangeNewMaxVolume(5));
            DecreaseVolume = new RelayCommand(() => ChangeNewMaxVolume(-5));

            // delta is how much mousewheel was moved
            // divide by ten because it's too big otherwise
            ChangeVolumeCommand = new RelayCommand<MouseWheelEventArgs>((e) =>
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
