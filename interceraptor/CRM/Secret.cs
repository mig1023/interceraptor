using System;
using System.IO;

namespace interceraptor.CRM
{
    class Secret
    {
        public const string Version = "4.0";

        private static string IP
        {
            get
            {
                return __TemporaryReadInDebugFile("[BASE]");
            }
        }

        public static string AuthenticationPath
        {
            get
            {
                //return IP + "idp/login?login=string&password=string";
                return IP + __TemporaryReadInDebugFile("[LOGIN]");
            }
        }

        public static string EchoPath
        {
            get
            {
                //return IP + "cashdesk/echo";
                return IP + __TemporaryReadInDebugFile("[ECHO]");
            }
        }


        public static string CashierPath
        {
            get
            {
                //return IP + "configurator/conf/user/";
                return IP + __TemporaryReadInDebugFile("[CONFIGURATOR]");
            }
        }

        public static string ServicesPath
        {
            get
            {
                //return IP + "dispatcher/config/?keys=";
                return IP + __TemporaryReadInDebugFile("[DISPATCHER]");
            }
        }

        public static string CaltulatePath
        {
            get
            {
                //return IP + "cashdesk/pay/service";
                return IP + __TemporaryReadInDebugFile("[PAY]");
            }
        }

        private static string __TemporaryReadInDebugFile(string type)
        {
            using (var reader = new StreamReader("debug.api.txt"))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line == type)
                        return reader.ReadLine();
                }
            }

            return string.Empty;
        }
    }
}
