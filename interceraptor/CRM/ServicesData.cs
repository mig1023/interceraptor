using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interceraptor.CRM
{
    class ServicesData
    {
        public string id { get; set; }

        public string name { get; set; }

        public string department { get; set; }

        public decimal price { get; set; }

        public int? group { get; set; }

        public int taxGroup { get; set; }

        public int qty { get; set; }

        public bool isPriceManual { get; set; }

        public bool isComment { get; set; }
    }
}
