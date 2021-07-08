using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuietTime.ViewModels;

namespace QuietTime.Services
{
    public class Navigator
    {
        private ViewModelBase? _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel!; }
            set { _currentViewModel = value; CurrentViewModelChanged?.Invoke(this, EventArgs.Empty); }
        }

        public event EventHandler? CurrentViewModelChanged;
    }
}
