using System;
using System.Net.Sockets;
using System.Text;

namespace interceraptor.Server
{
    class Client
    {
        public Client(TcpClient Client)
        {
            string request = String.Empty;

            byte[] buffer = new byte[1024];
            int count = 0;

            while ((count = Client.GetStream().Read(buffer, 0, buffer.Length)) > 0)
            {
                request += Encoding.ASCII.GetString(buffer, 0, count);

                if (request.IndexOf("\r\n\r\n") >= 0)
                    break;
            }

            request = Uri.UnescapeDataString(request);

            Client.Close();
        }
    }
}
