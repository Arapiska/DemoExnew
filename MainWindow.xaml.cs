using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DemoEx
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Content = new download();
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверенны, что хотите закрыть окно?", "Закрыть?", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No) {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }
    }
}
