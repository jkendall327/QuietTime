using QuietTime.Other;
using QuietTime.ViewModels;
using System.Windows;

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
