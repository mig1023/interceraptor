using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace interceraptor.Server
{
    class Client
    {
        public Client(HttpListenerRequest request, HttpListenerContext context)
        {
            Parser parser = Parser.Get();
            JObject json = parser.GetRequestBody(request);

            /// cashbox

            PayResponse dummyResponse = new PayResponse
            {
                checkNo = "1",
                cashChange = "0",
                error = new PayResponse.ErrorType
                {
                    type = "USER",
                    message = "ERR"
                },
            };

            Send(dummyResponse, context);
        }

        private async void Send(PayResponse data, HttpListenerContext context)
        {
            var connect = CRM.Connect.Get();
            var response = context.Response;

            var jsonData = JsonConvert.SerializeObject(data);
            byte[] responseBytes = Encoding.UTF8.GetBytes(jsonData);

            response.ContentLength64 = responseBytes.Length;
            response.Headers.Add("Content-Type", "application/json");
            response.Headers.Add("Authorization", $"Bearer {connect.Current.Token}");

            using (Stream output = response.OutputStream)
            {
                await output.WriteAsync(responseBytes, 0, responseBytes.Length);
                await output.FlushAsync();
            }
        }
    }
}
