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

namespace interceraptor.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ManuallyButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            ManuallyWindow manuallyWindow = new ManuallyWindow
            {
                Left = this.Left,
                Top = this.Top,
            };

            manuallyWindow.InitServicesTable();
            manuallyWindow.Owner = this;
            manuallyWindow.Show();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            ReportsWindow reportsWindow = new ReportsWindow
            {
                Left = this.Left,
                Top = this.Top,
            };

            reportsWindow.Owner = this;
            reportsWindow.Show();
        }
    }
}
