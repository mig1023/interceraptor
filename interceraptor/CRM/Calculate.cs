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

        public async Task<List<CRM.ServicesData>> DocPack(List<CRM.ServicesData> services)
        {
            string url = CRM.Secret.CaltulatePath;
            string response = String.Empty;
            Connect connect = Connect.Get();

            try
            {
                response = await Request.Send(url, services, withUserId: true);
            }
            catch (Exception)
            {
                return null;
            }

            List<CRM.ServicesData> pricedServices;

            try
            {
                pricedServices = JsonConvert.DeserializeObject<List<CRM.ServicesData>>(response);
            }
            catch (Exception)
            {
                return null;
            }

            return pricedServices;
        }
    }
}
