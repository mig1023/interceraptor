using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interceraptor.Create
{
    class DocPack
    {
        private static DocPack _singleton { get; set; }

        private List<CRM.ServicesData> _services { get; set; }

        private List<CRM.ServicesData> _servicesPriced { get; set; }

        private DocPack() { }

        public static DocPack Get()
        {
            if (_singleton == null)
            {
                _singleton = new DocPack();
            }

            return _singleton;
        }

        public void Init(List<CRM.ServicesData> services)
        {
            _services = new List<CRM.ServicesData>(services);
        }

        public async Task<bool> Calculate()
        {
            CRM.Calculate calculator = CRM.Calculate.Get();

            // prices from interface
            _servicesPriced = await calculator.DocPack(_services);

            return true;
        }

        public List<CRM.ServicesData> List()
        {
            return _servicesPriced;
        }

        public async Task<bool> Print()
        {
            return true;
        }
    }
}
