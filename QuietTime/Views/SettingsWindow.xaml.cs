using QuietTime.Other;
using QuietTime.ViewModels;
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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsWindowVM vm)
        {
            InitializeComponent();
            
            DataContext = vm;
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleValue = _scaleHelper.CalculateScale(ActualHeight, ActualWidth, MainStack);
        }

        // make the UI auto-scale when the window resizes
        private static readonly ScaleValueHelper<SettingsWindow> _scaleHelper = new();
        private static readonly DependencyProperty ScaleValueProperty = _scaleHelper.Get();

        public double ScaleValue
        {
            get => (double)GetValue(ScaleValueProperty);
            set => SetValue(ScaleValueProperty, value);
        }
    }
}
