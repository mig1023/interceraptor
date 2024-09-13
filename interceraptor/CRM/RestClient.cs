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

        private void AddHeaders(bool withUserId, bool withToken)
        {
            _httpClient.DefaultRequestHeaders.Clear();

            Connect connect = Connect.Get();

            if (withUserId)
                _httpClient.DefaultRequestHeaders.Add("UUID", connect.Current.UserId);

            if (withToken)
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {connect.Current.Token}");
        }

        public async Task<HttpResponseMessage> PostDataAsync(string url, object data,
            bool withUserId = false, bool withToken = false)
        {
            var jsonData = JsonConvert.SerializeObject(data);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            AddHeaders(withUserId, withToken);

            var response = await _httpClient.PostAsync(url, content);

            return response;
        }

        public async Task<HttpResponseMessage> GetDataAsync(string url,
            bool withUserId = false, bool withToken = false)
        {
            AddHeaders(withUserId, withToken);

            return await _httpClient.GetAsync(url);
        }
    }
}