using System;
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
            Logging.Main.Get().Log(string.Empty);
            Logging.Main.Get().Log("ЗАПУСК INTERCERAPTOR");
            Logging.Main.Get().Log(string.Empty);

            Waiting("Запуск внутреннего сервера...");

            Server.Listener.Start();

            Waiting("Подключение к кассе...");

            var cashbox = await Cashbox.Connect.Get();
            var error = String.Empty;

            if (!cashbox.Check(out error))
            {
                Output.MessageBoxes.Get().MessageBoxError($"Ошибка подключения к кассе:\n{error}");
                Disconnect();
                return;
            }

            Waiting("Подключение к серверу...");

            var server = CRM.Connect.Get();

            bool isConnected = await server.Authentication(Login.Text, Password.Password, String.Empty);

            if (!isConnected)
            {
                Output.MessageBoxes.Get().MessageBoxError(server.Current.Error);
                Disconnect();
                return;
            }

            Waiting("Проверка связи с сервером...");

            var echo = CRM.Echo.Get();

            bool isPingSuccess = await echo.Ping();

            if (!isPingSuccess)
            {
                Output.MessageBoxes.Get().MessageBoxError("Ошибка подключения к серверу:\nНет пинга");
                Disconnect();
                return;
            }

            echo.StartWoodpecker();

            Waiting("Загрузка данных с сервера...");

            var services = CRM.Services.Get();
            bool loaded = await services.Load();

            if (!loaded)
            {
                Output.MessageBoxes.Get().MessageBoxError("Ошибка получения данных с сервера или их неправильный формат");
                Disconnect();
                return;
            }

            Waiting("Установка данных кассира в кассе...");

            var cashier = CRM.Cashier.Get();
            var currentCashier = await cashier.Current();

            if (currentCashier.isLocked)
            {
                Output.MessageBoxes.Get().MessageBoxError("Ошибка установки данных кассира:\nКассир заблокирован в системе");
                Disconnect();
                return;
            }

            var setting = await Cashbox.Setting.Get();

            if (!setting.Cashier(currentCashier.cashier))
            {
                Output.MessageBoxes.Get().MessageBoxError("Ошибка установки данных кассира");
                Disconnect();
                return;
            }

            this.Hide();

            MainWindow mainWindow = new MainWindow
            {
                Left = this.Left,
                Top = this.Top,
            };

            Logging.Main.Get().Log("Запуск прошёл успешно");
            mainWindow.Show();
        }

        private void Disconnect()
        {
            //

            Wait.Visibility = Visibility.Hidden;
            LoginForm.Visibility = Visibility.Visible;
        }

        private void Waiting(string text)
        {
            LoginForm.Visibility = Visibility.Hidden;

            WaitFor.Content = text;

            if (Wait.Visibility != Visibility.Visible)
                Wait.Visibility = Visibility.Visible;

            Logging.Main.Get().Log(text);
        }

        private void WaitSpinner_MediaEnded(object sender, RoutedEventArgs e)
        {
            WaitSpinner.Position = new TimeSpan(0, 0, 1);
            WaitSpinner.Play();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Login.Focus();
        }
    }
}
