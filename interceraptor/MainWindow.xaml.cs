using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace interceraptor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Waiting("Подключение к кассе...");

            Cashbox.Connect cashbox = await Cashbox.Connect.Get();

            Waiting("Подключение к серверу...");

            CRM.Connect server = CRM.Connect.Get();

            // echo

            Wait.Visibility = Visibility.Hidden;
            LoginForm.Visibility = Visibility.Visible;
        }

        private async void LetsConnect_Click(object sender, RoutedEventArgs e)
        {
            Waiting("Логинимся...");

            CRM.Connect server = CRM.Connect.Get();

            bool isConnected = await server.Authentication(Login.Text, Password.Password, String.Empty);

            if (isConnected)
            {
                Wait.Visibility = Visibility.Hidden;

                MessageBox.Show($"Success connected!\n\nToken: " + server.Current.Token +
                    "\n\nUserId: " + server.Current.UserId + "\n\nRefreshToken: " + server.Current.RefreshToken);
            }
            else
            {
                Wait.Visibility = Visibility.Hidden;

                MessageBox.Show("Error: " + server.Current.Error);
            }
        }

        private void Waiting(string text)
        {
            WaitFor.Content = text;

            if (Wait.Visibility != Visibility.Visible)
                Wait.Visibility = Visibility.Visible;
        }

        private void WaitSpinner_MediaEnded(object sender, RoutedEventArgs e)
        {
            WaitSpinner.Position = new TimeSpan(0, 0, 1);
            WaitSpinner.Play();
        }
    }
}
