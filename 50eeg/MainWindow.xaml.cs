using _50eeg.UserController;
using _50eeg.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _50eeg
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new MainViewModel();
            
            InitializeComponent();
            scichart demo  = new scichart();
            UC_SciChart.Children.Add(demo);
        }
        //private void Window_loaded(object sender,RoutedEventArgs e)
        //{
            
        //}
    }
}
