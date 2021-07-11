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
    public class TrayIconVM : AudioAwareBaseVM
    {
        public TrayIconVM(IAudioLocker audio) : base(audio)
        {

            ShowAppCommand = new RelayCommand(() => ShowAppRequested?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler? ShowAppRequested;
        public ICommand ShowAppCommand { get; set; }
    }
}
