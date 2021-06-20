using QuietTime.ViewModels;
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
        }
    }
}
