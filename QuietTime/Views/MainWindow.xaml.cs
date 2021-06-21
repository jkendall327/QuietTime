using QuietTime.ViewModels;
using System;
using System.Windows;

namespace QuietTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowVM vm)
        {
            InitializeComponent();

            DataContext = vm;

            TrayIcon.Icon = new("icon.ico");
        }

        /*
         * Source for scaling UI: https://www.inchoatethoughts.com/scaling-your-user-interface-in-a-wpf-application
         * I don't fully understand this, I'm not sure I want to fully understand this, but it seems to work OK.
         */

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            double yScale = ActualHeight / 400.0d;
            double xScale = ActualWidth / 500.0d;
            double value = Math.Min(xScale, yScale);
            ScaleValue = (double)OnCoerceScaleValue(MainGrid, value);
        }

        #region ScaleValue Depdency Property

        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register(
            "ScaleValue", typeof(double), typeof(MainWindow),
            new UIPropertyMetadata(
                1.0,
                new PropertyChangedCallback(OnScaleValueChanged),
                new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            if (o is MainWindow mainWindow)
            {
                return mainWindow.OnCoerceScaleValue((double)value);
            }

            return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0d;

            return Math.Max(0.1, value);
        }

        public double ScaleValue
        {
            get => (double)GetValue(ScaleValueProperty);
            set => SetValue(ScaleValueProperty, value);
        }

        #endregion
    }
}
