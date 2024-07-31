using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace interceraptor.Server
{
    class Listner
    {
        private TcpListener _listener { get; set; }

        private static readonly BackgroundWorker _server = new BackgroundWorker();

        public Listner(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), _listener.AcceptTcpClient());
            }
        }

        private void ClientThread(object state)
        {
            string endPoint = (state as TcpClient).Client.RemoteEndPoint.ToString();

            if (!Secret.IsAuthorised(endPoint))
                return;

            new Client((TcpClient)state);
        }

        public static void Start()
        {
            _server.DoWork += Lister;
            _server.RunWorkerAsync();
        }

        private static void Lister(object sender, DoWorkEventArgs e)
        {
            new Listner(Secret.Port);
        }

        ~Listner()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
