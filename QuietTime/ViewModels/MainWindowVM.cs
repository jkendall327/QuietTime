using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NAudio.CoreAudioApi;
using Quartz;
using Quartz.Impl;
using QuietTime.Models;
using QuietTime.Other;
using QuietTime.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuietTime.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        public int CurrentVolume
        {
            get { return _currentVolume; }
            set { SetProperty(ref _currentVolume, value); }
        }
        private int _currentVolume;

        public int MaxVolume
        {
            get { return _maxVolume; }
            set { SetProperty(ref _maxVolume, value); }
        }
        private int _maxVolume;

        public int NewMaxVolume
        {
            get { return _newMaxVolume; }
            set { SetProperty(ref _newMaxVolume, value); }
        }
        private int _newMaxVolume;

        public string VersionInfo
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

                return $"v{version}, built {buildDate:d}";
            }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            set { SetProperty(ref _isLocked, value); }
        }
        private bool _isLocked;

        // commands
        public RelayCommand LockVolume { get; set; }

        public AudioService Audio { get; }

        public MainWindowVM(ILogger<MainWindowVM> logger, AudioService audio)
        {
            logger.LogInformation(new EventId(0, "Startup"), "App booted succesfully.");

            Audio = audio;

            LockVolume = new(() =>
            {
                audio.SwitchLock(NewMaxVolume);
                MaxVolume = NewMaxVolume;
            });

            Audio.LockStatusChanged += (s, e) => IsLocked = e;
            Audio.VolumeChanged += (s, e) => CurrentVolume = e;

            // get current volume for UI before any updates
            CurrentVolume = audio.CurrentVolume;
        }
    }
}
