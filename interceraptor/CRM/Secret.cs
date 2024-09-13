using System;

namespace interceraptor.CRM
{
    class Secret
    {
        public const string Version = "4.0";

        private const string IP = "127.0.0.1";

        public const string AuthenticationPath = IP + "idp/login?login=string&password=string";

        public const string EchoPath = IP + "cashdesk/echo";

        public const string CashierPath = IP + "configurator/conf/user/";

        public const string ServicesPath = IP + "dispatcher/config/?keys=";

        public const string CaltulatePath = IP + "cashdesk/pay/service";
    }
}
