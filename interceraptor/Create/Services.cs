using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interceraptor.Create
{
    class Services
    {
        private static Services _singleton { get; set; }

        private Dictionary<string, CRM.ServicesData> _services { get; set; }

        public List<CRM.ServicesData> List { get { return _services.Values.ToList(); } }

        private Services() { }

        public static Services Get()
        {
            if (_singleton == null)
            {
                _singleton = new Services();
            }

            return _singleton;
        }

        public void Init()
        {
            _services = new Dictionary<string, CRM.ServicesData>();
        }

        public CRM.ServicesData Properties(string id)
        {
            if (_services.ContainsKey(id))
            {
                return _services[id];
            }
            else
            {
                var crmServices = CRM.Services.Get();
                return crmServices.Dictionary[id];
            }
        }

        public void Add(string id)
        {
            if (_services.ContainsKey(id))
            {
                _services[id].qty += 1;
            }
            else
            {
                var crmServices = CRM.Services.Get();
                var service = crmServices.Dictionary[id];
                service.qty = 1;
                _services.Add(service.id, service);
            }
        }

        public void Add(string id, string price, string comment)
        {
            if (!_services.ContainsKey(id))
            {
                var crmServices = CRM.Services.Get();
                var service = crmServices.Dictionary[id];
                service.qty = 1;
                _services.Add(service.id, service);
            }

            _services[id].price = Decimal.Parse(price);
            _services[id].comment = comment;
        }

        public void Sub(string id)
        {
            if (!_services.ContainsKey(id))
                return;

            if (_services[id].qty > 0)
                _services[id].qty -= 1;

            if (_services[id].qty <= 0)
                _services.Remove(id);
        }

        public int Count(string id)
        {
            if (_services.ContainsKey(id))
            {
                return _services[id].qty;
            }
            else
            {
                return 0;
            }
        }
    }
}
