using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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

        public MainPageVM(ILogger<MainPageVM> logger, AudioService audio) : base(audio)
        {
            logger.LogInformation(EventIds.AppStartup, "App booted succesfully.");

            IncreaseVolume = new RelayCommand(() => ChangeNewMaxVolume(5));
            DecreaseVolume = new RelayCommand(() => ChangeNewMaxVolume(-5));

            LockVolume = new RelayCommand(() => audio.SwitchLock(NewMaxVolume));

            audio.LockStatusChanged += (s, e) => IsLocked = e;
        }

        public void ChangeNewMaxVolume(int amount)
        {
            NewMaxVolume += amount;
            NewMaxVolume = Math.Clamp(NewMaxVolume, 0, 100);
        }
    }
}
