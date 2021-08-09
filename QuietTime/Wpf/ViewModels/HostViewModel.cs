using QuietTime.Navigation;

namespace QuietTime.Wpf.ViewModels
{
    internal class HostViewModel : ViewModelBase
    {
        Navigator _navigator;

        public HostViewModel(Navigator navigator, NavigationBarVM navigationBarViewModel)
        {
            _navigator = navigator;
            _navigationBarVM = navigationBarViewModel;

            _navigator.CurrentViewModelChanged += (s, e) => OnPropertyChanged(nameof(CurrentViewModel));
        }

        public ViewModelBase CurrentViewModel => _navigator.CurrentViewModel;

        private NavigationBarVM _navigationBarVM;
        public NavigationBarVM NavigationBarViewModel
        {
            get { return _navigationBarVM; }
            set { _navigationBarVM = value; }
        }
    }
}
