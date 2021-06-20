using Microsoft.Toolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using QuietTime.Other;

namespace QuietTime.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        readonly MMDevice _device;

        private int _currentVolume;

        public int CurrentVolume
        {
            get { return _currentVolume; }
            set { SetProperty(ref _currentVolume, value); }
        }

        private int _maxVolume = 50;

        public int MaxVolume
        {
            get { return _maxVolume; }
            set { SetProperty(ref _maxVolume, value); }
        }


        public MainWindowVM(MMDeviceEnumerator enumerator)
        {
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;

            CurrentVolume = _device.AudioEndpointVolume.MasterVolumeLevelScalar.ToPercentage();
        }

        private void OnVolumeChange(AudioVolumeNotificationData data)
        {
            var volume = data.MasterVolume.ToPercentage();

            if (volume > MaxVolume)
            {
                float newLevel = (float)MaxVolume / 100;
                _device.AudioEndpointVolume.MasterVolumeLevelScalar = newLevel;
            }

            CurrentVolume = volume;
        }
    }
}
