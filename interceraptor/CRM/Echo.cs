using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace interceraptor.CRM
{
    class Echo
    {
        private const int _oneMinute = 60000;

        private static Echo _singleton { get; set; }

        private Timer _timer;

        private Echo() { }

        public static Echo Get()
        {
            if (_singleton == null)
                _singleton = new Echo();

            return _singleton;
        }

        public async Task<bool> Ping()
        {
            string url = Secret.EchoPath;
            string initEcho = String.Empty;
            Connect server = Connect.Get();
            Cashbox.Connect cashbox = await Cashbox.Connect.Get();

            EchoData data = new EchoData
            {
                userId = server.Current.UserId,
                version = Secret.Version,
                serial = cashbox.Data.SerialNumber,
                ip = server.CurrentIP(),
            };

            try
            {
                initEcho = await Request.Send(url, data);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void StartWoodpecker()
        {
            _timer = new Timer(Woodpecker, null, 0, _oneMinute);
        }

        private async void Woodpecker(object state)
        {
            Task.Run(async () => { await Ping(); });
        }
    }
}
