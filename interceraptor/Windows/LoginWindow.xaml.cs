using interceraptor.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace interceraptor.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LetsConnect_Click(object sender, RoutedEventArgs e)
        {
            Waiting("Запуск внутреннего сервера...");

            Server.Listener.Start();

            Waiting("Подключение к кассе...");

            var cashbox = await Cashbox.Connect.Get();
            var error = String.Empty;

            if (!cashbox.Check(out error))
            {
                MessageBox.Show($"Ошибка подключения к кассе:\n{error}");
                Disconnect();
                return;
            }

            Waiting("Подключение к серверу...");

            var server = CRM.Connect.Get();

            bool isConnected = await server.Authentication(Login.Text, Password.Password, String.Empty);

            if (!isConnected)
            {
                MessageBox.Show(server.Current.Error);
                Disconnect();
                return;
            }

            Waiting("Проверка связи с сервером...");

            var echo = CRM.Echo.Get();

            bool isPingSuccess = await echo.Ping();

            if (!isPingSuccess)
            {
                MessageBox.Show($"Ошибка подключения к серверу:\nНет пинга");
                Disconnect();
                return;
            }

            echo.StartWoodpecker();

            Waiting("Загрузка данных с сервера...");

            var services = CRM.Services.Get();
            bool loaded = await services.Load();

            if (!loaded)
            {
                MessageBox.Show($"Ошибка получения данных с сервера или их неправильный формат");
                Disconnect();
                return;
            }

            Waiting("Установка данных кассира в кассе...");

            var cashier = CRM.Cashier.Get();
            var currentCashier = await cashier.Current();

            if (currentCashier.isLocked)
            {
                MessageBox.Show($"Ошибка установки данных кассира:\nКассир заблокирован в системе");
                Disconnect();
                return;
            }

            var setting = await Cashbox.Setting.Get();

            if (!setting.Cashier(currentCashier.cashier))
            {
                MessageBox.Show($"Ошибка установки данных кассира");
                Disconnect();
                return;
            }

            this.Hide();

            MainWindow mainWindow = new MainWindow
            {
                Left = this.Left,
                Top = this.Top,
            };

            mainWindow.Show();
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
