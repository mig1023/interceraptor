using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interceraptor.Create
{
    class Parsing
    {
        private static Parsing _singleton { get; set; }

        private Parsing() { }

        public static Parsing Get()
        {
            if (_singleton == null)
            {
                _singleton = new Parsing();
            }

            return _singleton;
        }

        public bool Price(string line, out double price) =>
            double.TryParse(line, out price);

        public CRM.ServicesData Service(string line)
        {
            return null;
        }
    }
}
