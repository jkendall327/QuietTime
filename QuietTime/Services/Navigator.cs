using System;
using QuietTime.ViewModels;

namespace QuietTime.Services
{
    /// <summary>
    /// Allows navigation between viewmodels by storing the app's current viewmodel.
    /// </summary>
    internal class Navigator
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
