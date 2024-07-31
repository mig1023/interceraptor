using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;

namespace interceraptor.CRM
{
    class Connect
    {
        private static Connect _singleton { get; set; }

        public ConnectData Current { get; set; }

        private Connect() { }

        public static Connect Get()
        {
            if (_singleton == null)
            {
                _singleton = new Connect();
                _singleton.Current = new ConnectData();
            }

            return _singleton;
        }

        private bool Error(string text)
        {
            Current.Error = text;
            return false;
        }

        public async Task<bool> Authentication(string login, string passwordLine, string serialNo)
        {
            string url = Secret.AuthenticationPath;
            string authentication = String.Empty;
            Current.IsConnected = false;

            try
            {
                authentication = await Request.Send(url, new LoginData { login = login, password = passwordLine });
            }
            catch (Exception)
            {
                return Error("Ошибка подключения к серверу");
            }

            JObject data;

            try
            {
                data = JObject.Parse(authentication);
            }
            catch (Exception)
            {
                return Error("Ошибка данных, полученых от сервера");
            }

            if (!String.IsNullOrEmpty(data["message"].ToString()))
            {
                return Error("Ошибка от сервера:\n" + data["message"].ToString());
            }

            try
            {
                Current.Token = data["data"]["token"].ToString();
                Current.UserId = data["data"]["id"].ToString();
                Current.RefreshToken = data["data"]["refreshToken"].ToString();
            }
            catch (Exception)
            {
                return Error("Ошибка разбора данных, полученых от сервера");
            }

            Current.IsConnected = false;
            return true;
        }

        public string CurrentIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            return String.Empty;
        }
    }
}
