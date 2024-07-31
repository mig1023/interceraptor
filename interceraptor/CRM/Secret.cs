using System;

namespace interceraptor.CRM
{
    class Secret
    {
        public const string Version = "4.0";

        private const string IP = "127.0.0.1";

        public const string AuthenticationPath = IP + ":80/login?login=string&password=string";

        public const string EchoPath = IP + ":80/echo";

        public const string CashierPath = IP + ":80/conf/user/";

        public const string ServicesPath = IP + ":80/?keys=";
    }
}
