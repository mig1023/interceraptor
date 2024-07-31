using System;
using System.Windows;

namespace interceraptor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LetsConnect_Click(object sender, RoutedEventArgs e)
        {
            Waiting("Запуск сервера...");

            Server.Listener.Start();

            Waiting("Подключение к кассе...");

            Cashbox.Connect cashbox = await Cashbox.Connect.Get();

            Waiting("Подключение к серверу...");

            CRM.Connect server = CRM.Connect.Get();

            bool isConnected = await server.Authentication(Login.Text, Password.Password, String.Empty);

            if (!isConnected)
            {
                MessageBox.Show("Error: " + server.Current.Error);
                return;
            }

            CRM.Echo echo = CRM.Echo.Get();

            bool isPingSuccess = await echo.Ping();

            if (!isPingSuccess)
            {
                MessageBox.Show("Server ping error!");
            }

            echo.StartWoodpecker();

            Waiting("Interceraptor работает");
        }

        private void Waiting(string text)
        {
            LoginForm.Visibility = Visibility.Hidden;

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
