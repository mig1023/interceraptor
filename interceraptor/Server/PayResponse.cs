using System;

namespace interceraptor.Server
{
    class PayResponse
    {
        public string checkNo { get; set; }

        public string cashChange { get; set; }

        public ErrorType error { get; set; }

        public class ErrorType
        {
            public string type { get; set; }

            public string message { get; set; }
        }
    }
}
