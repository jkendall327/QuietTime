using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Core.Services;

namespace QuietTime.ViewModels
{
    /// <summary>
    /// Provides properties for <see cref="TrayIconVM"/> and <see cref="MainPageVM"/>.
    /// </summary>
    internal abstract class AudioAwareBaseVM : ViewModelBase
    {
        protected IAudioLocker _audio;

        protected AudioAwareBaseVM(IAudioLocker audio)
        {
            _audio = audio;
            audio.VolumeChanged += (s, e) => OnPropertyChanged(nameof(CurrentVolume));
            audio.MaxVolumeChanged += (s, e) => OnPropertyChanged(nameof(MaxVolume));

            CloseAppCommand = new RelayCommand(() => CloseAppRequested?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler? CloseAppRequested;

        public int CurrentVolume => _audio.CurrentVolume;
        public int MaxVolume => _audio.MaxVolume;

        public ICommand CloseAppCommand { get; set; }
    }
}
