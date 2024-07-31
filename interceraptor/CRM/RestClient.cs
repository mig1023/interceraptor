﻿using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

namespace interceraptor.CRM
{
    public class RestClient
    {
        private readonly HttpClient _httpClient;

        public RestClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> PostDataAsync(string url, object data)
        {
            var jsonData = JsonConvert.SerializeObject(data);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            return response;
        }
    }
}