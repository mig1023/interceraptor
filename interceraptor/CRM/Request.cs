using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interceraptor.CRM
{
    class Request
    {
        private static RestClient _restClient = new RestClient();

        public static async Task<String> Send(string url, object data)
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
    }
}
