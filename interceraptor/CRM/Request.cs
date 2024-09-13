using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace interceraptor.CRM
{
    class Request
    {
        private static RestClient _restClient = new RestClient();

        public static async Task<String> Send(string url, object data = null,
            bool withUserId = false, bool withToken = false)
        {
            HttpResponseMessage response;

            if (data == null)
            {
                response = await _restClient.GetDataAsync(url, withUserId: withUserId, withToken: withToken);
            }
            else
            {
                response = await _restClient.PostDataAsync(url, data, withUserId: withUserId, withToken: withToken);
            }

            if (!response.IsSuccessStatusCode)
            {
                return String.Empty;
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
