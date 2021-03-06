using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Core.AudioLocking;

namespace QuietTime.Wpf.ViewModels
{
    internal class TrayIconVM : AudioAwareBaseVM
    {
        public TrayIconVM(IAudioLocker audio) : base(audio)
        {

            ShowAppCommand = new RelayCommand(() => ShowAppRequested?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler? ShowAppRequested;
        public ICommand ShowAppCommand { get; set; }
    }
}
