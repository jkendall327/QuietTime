﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using QuietTime.Services;

namespace QuietTime.ViewModels
{
    public class ViewModelBase : ObservableObject
    { }

    public class HostViewModel : ViewModelBase
    {
        Navigator _navigator;

        public HostViewModel(Navigator navigator, NavigationBarVM navigationBarViewModel)
        {
            _navigator = navigator;
            _navigationBarVM = navigationBarViewModel;

            _navigator.CurrentViewModelChanged += (s, e) => OnPropertyChanged(nameof(CurrentViewModel));

            CloseAppCommand = new RelayCommand(() => CloseAppRequested?.Invoke(this, EventArgs.Empty));
        }

        public ViewModelBase CurrentViewModel => _navigator.CurrentViewModel;

        private NavigationBarVM _navigationBarVM;
        public NavigationBarVM NavigationBarViewModel
        {
            get { return _navigationBarVM; }
            set { _navigationBarVM = value; }
        }

        public ICommand CloseAppCommand { get; set; }

        public event EventHandler CloseAppRequested;
        public event EventHandler HideAppRequested;
        public event EventHandler RestoreFromTrayRequested;
    }
}