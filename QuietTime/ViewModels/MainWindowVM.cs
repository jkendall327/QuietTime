using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Other;
using QuietTime.Services;
using System;
using System.IO;
using System.Reflection;

namespace QuietTime.ViewModels
{
    /// <summary>
    /// A viewmodel for the main window.
    /// </summary>
    public class MainWindowVM : ObservableObject
    {
        /// <summary>
        /// The system's current volume.
        /// </summary>
        public int CurrentVolume
        {
            get { return _currentVolume; }
            set { SetProperty(ref _currentVolume, value); }
        }
        private int _currentVolume;

        /// <summary>
        /// The current volume limit.
        /// </summary>
        public int MaxVolume
        {
            get { return _maxVolume; }
            set { SetProperty(ref _maxVolume, value); }
        }
        private int _maxVolume;

        /// <summary>
        /// What <see cref="MaxVolume"/> will be set to when volume levels are locked.
        /// </summary>
        public int NewMaxVolume
        {
            get { return _newMaxVolume; }
            set { SetProperty(ref _newMaxVolume, value); }
        }
        private int _newMaxVolume;

        /// <summary>
        /// The version and build time of the program.
        /// </summary>
        public string VersionInfo
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

                return $"v{version}, built {buildDate:d}";
            }
        }

        /// <summary>
        /// Whether system audio is capped at <see cref="MaxVolume"/>.
        /// </summary>
        public bool IsLocked
        {
            get { return _isLocked; }
            set { SetProperty(ref _isLocked, value); }
        }
        private bool _isLocked;

        /// <summary>
        /// Encapsulates locking the system volume.
        /// </summary>
        public RelayCommand LockVolume { get; set; }

        /// <summary>
        /// Increases <see cref="NewMaxVolume"/> by 5.
        /// </summary>
        public RelayCommand IncreaseVolume { get; set; }

        /// <summary>
        /// Decreases <see cref="NewMaxVolume"/> by 5.
        /// </summary>
        public RelayCommand DecreaseVolume { get; set; }

        /// <summary>
        /// Creates a new <see cref="MainWindowVM"/>.
        /// </summary>
        /// <param name="logger">Logging framework for this class.</param>
        /// <param name="audio">Link to underlying audio infrastructure.</param>
        public MainWindowVM(ILogger<MainWindowVM> logger, AudioService audio)
        {
            logger.LogInformation(EventIds.AppStartup, "App booted succesfully.");

            IncreaseVolume = new RelayCommand(() => ChangeNewMaxVolume(5));
            DecreaseVolume = new RelayCommand(() => ChangeNewMaxVolume(-5));

            LockVolume = new(() =>
            {
                audio.SwitchLock(NewMaxVolume);
                MaxVolume = NewMaxVolume;
            });

            audio.LockStatusChanged += (s, e) => IsLocked = e;
            audio.VolumeChanged += (s, e) => CurrentVolume = e;
            audio.MaxVolumeChanged += (s, e) => MaxVolume = e;

            // get current volume for UI before any updates
            CurrentVolume = audio.CurrentVolume;
        }

        private void ChangeNewMaxVolume(int amount)
        {
            NewMaxVolume += amount;
            NewMaxVolume = Math.Clamp(NewMaxVolume, 0, 100);
        }
    }
}
