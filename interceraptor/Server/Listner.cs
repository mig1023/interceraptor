using System;
using System.ComponentModel;
using System.Net;

namespace interceraptor.Server
{
    class Listener
    {
        private HttpListener _listener { get; set; }

        private static readonly BackgroundWorker _server = new BackgroundWorker();

        public Listener(int port)
        {
            var connect = CRM.Connect.Get();

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://{connect.CurrentIP()}:{port}/");
            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            _listener.Start();

            while (true)
            {
                var result = _listener.BeginGetContext(new AsyncCallback(ClientThread), _listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        private void ClientThread(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            var request = context.Request;

            string ip = request.RemoteEndPoint.Address.ToString();

            if (!Secret.IsAuthorised(ip))
                return;

            if (!request.HasEntityBody)
                return;

            new Client(request, context);
        }

        public static void Start()
        {
            _server.DoWork += DoListener;
            _server.RunWorkerAsync();
        }

        private static void DoListener(object sender, DoWorkEventArgs e)
        {
            new Listener(Secret.Port);
        }

        ~Listener()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
