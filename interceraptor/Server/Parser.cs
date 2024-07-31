using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace interceraptor.Server
{
    class Parser
    {
        private static Parser _singleton { get; set; }

        private Parser() { }

        public static Parser Get()
        {
            if (_singleton == null)
            {
                _singleton = new Parser();
            }

            return _singleton;
        }

        public JObject GetRequestBody(HttpListenerRequest request)
        {
            string line;

            using (Stream body = request.InputStream)
            {
                using (var reader = new StreamReader(body, request.ContentEncoding))
                {
                    line = reader.ReadToEnd();
                }
            }

            JObject data;

            try
            {
                data = JObject.Parse(line);
            }
            catch (Exception)
            {
                return null;
            }

            return data;
        }
    }
}
