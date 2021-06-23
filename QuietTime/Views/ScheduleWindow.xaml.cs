﻿using QuietTime.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuietTime.Views
{
    /// <summary>
    /// Interaction logic for ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        /// <summary>
        /// Creates a new <see cref="ScheduleWindow"/>.
        /// </summary>
        /// <param name="vm">The viewmodel for this window.</param>
        public ScheduleWindow(ScheduleWindowVM vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}
