using System;
using System.Windows;

namespace interceraptor.Output
{
    class MessageBoxes
    {
        private static MessageBoxes _singleton { get; set; }

        private MessageBoxes() { }

        public static MessageBoxes Get()
        {
            if (_singleton == null)
            {
                _singleton = new MessageBoxes();
            }

            return _singleton;
        }

        private MessageBoxResult MessageBoxYesNo(string message, bool warning = false)
        {
            Logging.Main.Get().Log($"ЗАПРОС: {message}");

            MessageBoxResult result = MessageBox.Show(message, "Внимание!",
                MessageBoxButton.YesNo, (warning ? MessageBoxImage.Warning : MessageBoxImage.Question));

            var response = result == MessageBoxResult.Yes ? "ДА" : "нет";
            Logging.Main.Get().Log($"ОТВЕТ: {response}");

            return result;
        }

        public MessageBoxResult MessageBoxError(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Ошибка!",
                MessageBoxButton.OK, MessageBoxImage.Error);

            Logging.Main.Get().Log($"Сообщение об ошибке: {message}");

            return result;
        }

        public MessageBoxResult ReportCleaning() =>
            _singleton.MessageBoxYesNo("Распечатка отчёта с гашением приведёт к закрытию смены. Продолжить?", warning: true);
    }
}
