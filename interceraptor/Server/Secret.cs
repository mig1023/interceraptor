using System;
using System.Collections.Generic;

namespace interceraptor.Server
{
    class Secret
    {
        public static int Port = 80;

        private static List<string> IpWhiteList = new List<string>
        {
            "127.0.0.1",
        };

        public static bool IsAuthorised(string ip)
        {
            string[] ipAddr = ip.Split(':');
            return IpWhiteList.Contains(ipAddr[0]);
        }
    }
}
