using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interceraptor.CRM
{
    class ConnectData
    {
        public string Token { get; set; }

        public string UserId { get; set; }

        public string RefreshToken { get; set; }

        public bool IsConnected { get; set; }

        public string Error { get; set; }
    }
}
