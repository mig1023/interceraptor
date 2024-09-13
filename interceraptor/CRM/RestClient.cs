using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace interceraptor.CRM
{
    public class RestClient
    {
        private readonly HttpClient _httpClient;

        public RestClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> PostDataAsync(string url, object data, bool withUserId = false)
        {
            var jsonData = JsonConvert.SerializeObject(data);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            if (withUserId)
            {
                IEnumerable<string> _;

                if (!_httpClient.DefaultRequestHeaders.TryGetValues("UUID", out _))
                {
                    Connect connect = Connect.Get();
                    _httpClient.DefaultRequestHeaders.Add("UUID", connect.Current.UserId);
                }
            }

            var response = await _httpClient.PostAsync(url, content);

            return response;
        }

        public async Task<HttpResponseMessage> GetDataAsync(string url)
        {
            return await _httpClient.GetAsync(url);
        }
    }
}