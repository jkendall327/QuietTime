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

        // make the UI auto-scale when the window resizes
        private static readonly ScaleValueHelper<ScheduleWindow> _scaleHelper = new();
        private readonly DependencyProperty ScaleValueProperty = _scaleHelper.Get();

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleValue = _scaleHelper.CalculateScale(ActualHeight, ActualWidth, MainGrid);
        }

        /// <summary>
        /// How this window scales when resized.
        /// </summary>
        public double ScaleValue
        {
            get => (double)GetValue(ScaleValueProperty);
            set => SetValue(ScaleValueProperty, value);
        }
    }
}
