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

        public MainWindowVM(MMDeviceEnumerator enumerator)
        {
            _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeChange;

            CurrentVolume = _device.AudioEndpointVolume.MasterVolumeLevelScalar.ToPercentage();
        }

        private void OnVolumeChange(AudioVolumeNotificationData data)
        {
            CurrentVolume = data.MasterVolume.ToPercentage();
        }
    }
}
