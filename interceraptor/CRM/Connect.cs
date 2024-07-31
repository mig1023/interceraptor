using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace interceraptor.CRM
{
    class Connect
    {
        private RestClient _restClient { get; set; }

        private static Connect _singleton { get; set; }

        public ConnectionData Current { get; set; }

        private Connect() { }

        public static Connect Get()
        {
            if (_singleton == null)
            {
                _singleton = new Connect();
                _singleton.Current = new ConnectionData();
                _singleton._restClient = new RestClient();
            }

            return _singleton;
        }

        private async Task<String> SendLogin(string url, object data)
        {
            var response = await _restClient.PostDataAsync(url, data);

            if (!response.IsSuccessStatusCode)
            {
                return String.Empty;
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        private bool Error(string text)
        {
            Current.Error = text;
            return false;
        }

        public async Task<bool> Authentication(string login, string passwordLine, string serialNo)
        {
            string url = Secret.authPath;
            string authentication = String.Empty;
            Current.IsConnected = false;

            try
            {
                authentication = await SendLogin(url, new LoginData { Login = login, Password = passwordLine });
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

            if (String.IsNullOrEmpty(data["data"].ToString()))
            {
                return Error("Ошибка от сервера: " + data["message"].ToString());
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
    }
}
