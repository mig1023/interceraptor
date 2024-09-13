using Newtonsoft.Json.Linq;
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

        public async Task<bool> Calculate(string rgs, string kl, string fox)
        {
            CRM.Calculate calculator = CRM.Calculate.Get();

            // Parsing.Service

            _servicesPriced = await calculator.DocPack(_services);

            return true;
        }

        public List<CRM.ServicesData> List()
        {
            return _servicesPriced;
        }

        public async Task<Server.PayResponse> Print(string email)
        {
            JObject docPack = JObject.FromObject(new
            {
                agreement = new
                {
                    services = _servicesPriced,
                },
                customer = new
                {
                    email = email,
                },
                cashTotal = 100,
                payMethod = "CREDIT_CARD",
            });

            Cashbox.Printer cashbox = Cashbox.Printer.Get();
            return cashbox.Print(docPack);
        }
    }
}
