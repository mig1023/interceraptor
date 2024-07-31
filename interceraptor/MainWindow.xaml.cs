using System;
using System.Threading;
using System.Threading.Tasks;
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
            Waiting("Запуск внутреннего сервера...");

            Server.Listener.Start();

            Waiting("Подключение к кассе...");

            var cashbox = await Cashbox.Connect.Get();

            if (!cashbox.Check())
            {
                MessageBox.Show("Cashbox connection error!");
                Disconnect();
                return;
            }

            Waiting("Подключение к серверу...");

            var server = CRM.Connect.Get();

            bool isConnected = await server.Authentication(Login.Text, Password.Password, String.Empty);

            if (!isConnected)
            {
                MessageBox.Show("Error: " + server.Current.Error);
                Disconnect();
                return;
            }

            Waiting("Проверка связи с сервером...");

            var echo = CRM.Echo.Get();

            bool isPingSuccess = await echo.Ping();

            if (!isPingSuccess)
            {
                MessageBox.Show("Server ping error!");
                Disconnect();
                return;
            }

            echo.StartWoodpecker();

            Waiting("Загрузка данных с сервера...");

            var services = CRM.Services.Get();
            bool loaded = await services.Load();

            if (!loaded)
            {
                MessageBox.Show("Services loading error!");
                Disconnect();
                return;
            }

            Waiting("Установка данных кассира в кассе...");

            var cashier = CRM.Cashier.Get();
            var currentCashier = await cashier.Current();

            if (currentCashier.isLocked)
            {
                MessageBox.Show("Cashiers locked!");
                Disconnect();
                return;
            }

            var setting = await Cashbox.Setting.Get();

            if (!setting.Cashier(currentCashier.cashier))
            {
                MessageBox.Show("Cashiers error!");
                Disconnect();
                return;
            }

            Waiting("Interceraptor работает");
        }

        private void Disconnect()
        {
            //
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
