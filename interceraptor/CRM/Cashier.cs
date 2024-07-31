using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace interceraptor.CRM
{
    class Cashier
    {
        private static Cashier _singleton { get; set; }

        private Cashier() { }

        public static Cashier Get()
        {
            if (_singleton == null)
                _singleton = new Cashier();

            return _singleton;
        }

        public async Task<CashierData> Current()
        {
            string url = Secret.CashierPath;
            string response = String.Empty;
            Connect connect = Connect.Get();

            try
            {
                response = await Request.Send(url + connect.Current.UserId);
            }
            catch (Exception)
            {
                return null;
            }

            JObject data;

            try
            {
                data = JObject.Parse(response);
            }
            catch (Exception)
            {
                return null;
            }

            var cashierData = new CashierData
            {
                cashier = data["cashdesk"]["name"].ToString(),
                isLocked = data["isLocked"].ToString() == "true",
            };

            return cashierData;
        }
    }
}
