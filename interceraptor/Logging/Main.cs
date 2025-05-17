using System;
using System.IO;

namespace interceraptor.Logging
{
    class Main
    {
        const string UPDATE_LOGS_DIR = "Logs";

        private static Main _singleton { get; set; }

        private Main() { }

        public static Main Get()
        {
            if (_singleton == null)
            {
                _singleton = new Main();
            }

            return _singleton;
        }

        public void Log(string line)
        {
            try
            {
                LogDirectory();

                var fileName = UPDATE_LOGS_DIR + "\\main.log";
                var dateLine = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                var log = line.Replace('\n', ' ');

                using (StreamWriter sw = new StreamWriter(fileName, true))
                {
                    sw.WriteLine($"{dateLine} {log}");
                }
            }
            finally
            {
                // nothing to do here
            }
        }

        private static void LogDirectory()
        {
            if (!Directory.Exists(UPDATE_LOGS_DIR))
                Directory.CreateDirectory(UPDATE_LOGS_DIR);
        }
    }
}
