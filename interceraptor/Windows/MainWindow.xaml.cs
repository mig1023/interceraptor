using System;
using System.Windows;

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
