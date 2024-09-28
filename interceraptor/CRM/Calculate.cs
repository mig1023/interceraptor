using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interceraptor.CRM
{
    class Calculate
    {
        private static Calculate _singleton { get; set; }

        private Calculate() { }

        public static Calculate Get()
        {
            if (_singleton == null)
                _singleton = new Calculate();

            return _singleton;
        }

        private List<ServicesRequestData> PackRequestData(List<ServicesData> services)
        {
            var servicesMinData = new List<ServicesRequestData>();

            foreach (var service in services)
            {
                var minData = new ServicesRequestData
                {
                    id = service.id,
                    qty = service.qty,
                    price = service.price,
                    comment = String.Empty,
                };

                servicesMinData.Add(minData);
            }

            return servicesMinData;
        }

        public async Task<List<ServicesData>> DocPack(List<ServicesData> services, string date)
        {
            string url = Secret.CaltulatePath;
            string response = String.Empty;
            Connect connect = Connect.Get();
            
            if (!String.IsNullOrEmpty(date))
            {
                url += $"/{date}";
            }

            try
            {
                response = await Request.Send(url, PackRequestData(services), withToken: true);
            }
            catch (Exception)
            {
                return null;
            }

            List<ServicesData> pricedServices;

            try
            {
                pricedServices = JsonConvert.DeserializeObject<List<ServicesData>>(response);
            }
            catch (Exception)
            {
                return null;
            }

            return pricedServices;
        }
    }
}
