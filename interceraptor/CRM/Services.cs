using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interceraptor.CRM
{
    class Services
    {
        private static Services _singleton { get; set; }

        private List<ServicesData> _services { get; set; }

        private Services() { }

        public static Services Get()
        {
            if (_singleton == null)
            {
                _singleton = new Services();
            }

            return _singleton;
        }

        private async Task<JObject> DataRequest(string key)
        {
            string url = Secret.ServicesPath + key;
            string data = String.Empty;
            Connect server = Connect.Get();

            try
            {
                data = await Request.Send(url);
            }
            catch (Exception)
            {
                return null;
            }

            JObject json;

            try
            {
                json = JObject.Parse(data);
            }
            catch (Exception)
            {
                return null;
            }

            return json;
        }

        private Dictionary<string, string> Sort(JObject list, string listName, string key, string value)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var token in list[listName])
            {
                dictionary.Add(token[key].ToString(), token[value].ToString());
            }

            return dictionary;
        }

        public async Task<bool> Load()
        {
            JObject items = await DataRequest("cashdeskitem");

            if (items == null)
                return false;

            var itemsDict = Sort(items, "cashdeskitem", "nomenclatureId", "groupId");

            JObject groups = await DataRequest("cashdeskitemgroup");

            if (groups == null)
                return false;

            var groupDict = Sort(groups, "cashdeskitemgroup", "id", "index");

            JObject services = await DataRequest("nomenclature");

            if (services == null)
                return false;

            var servicesList = new List<ServicesData>();

            foreach (var token in services["nomenclature"])
            {
                var service = new ServicesData
                {
                    id = token["id"].ToString(),
                    name = token["printLabel"].ToString()
                };

                if (itemsDict.ContainsKey(service.id))
                {
                    string group = groupDict[itemsDict[service.id]];
                    service.group = int.Parse(group);
                }

                servicesList.Add(service);
            }

            _services = servicesList;

            return true;
        }
    }
}
