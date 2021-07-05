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
        /// <summary>
        /// Creates a new <see cref="SettingsWindow"/>.
        /// </summary>
        /// <param name="vm">Viewmodel for this window.</param>
        public SettingsWindow(SettingsWindowVM vm)
        {
            InitializeComponent();
            
            DataContext = vm;
        }
    }
}
