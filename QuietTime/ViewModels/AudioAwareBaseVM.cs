using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Core.Services;
using QuietTime.Services;

namespace QuietTime.ViewModels
{
    /// <summary>
    /// Provides properties for <see cref="TrayIconVM"/> and <see cref="MainPageVM"/>.
    /// </summary>
    public abstract class AudioAwareBaseVM : ViewModelBase
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
