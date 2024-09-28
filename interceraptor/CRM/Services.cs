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

        public List<ServicesData> List { get { return _services; } }

        public Dictionary<string, ServicesData> Dictionary
        {
            get { return _services.ToDictionary(x => x.id, x => x); }
        }

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
                data = await Request.Send(url, withToken: true);
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

        private Dictionary<string, Dictionary<string, string>> Separator(JObject list,
            string listName, string key)
        {
            var dictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var token in list[listName])
            {
                var dict = token.ToObject<Dictionary<string, string>>();
                dictionary.Add(token[key].ToString(), dict);
            }

            return dictionary;
        }

        public async Task<bool> Load()
        {
            JObject itemsJson = await DataRequest("cashdeskitem");

            if (itemsJson == null)
                return false;

            var items = Separator(itemsJson, "cashdeskitem", "nomenclatureId");

            JObject groupsJson = await DataRequest("cashdeskitemgroup");

            if (groupsJson == null)
                return false;

            var groups = Separator(groupsJson, "cashdeskitemgroup", "id");

            JObject services = await DataRequest("nomenclature");

            if (services == null)
                return false;

            var servicesList = new List<ServicesData>();

            foreach (var token in services["nomenclature"])
            {
                if (token["isEnabled"].ToString() == "false")
                    continue;

                string id = token["id"].ToString();

                if (!items.ContainsKey(id))
                    continue;

                var service = new ServicesData
                {
                    id = items[id]["id"],
                    name = token["printLabel"].ToString()
                };

                if (groups.ContainsKey(service.id))
                {
                    string group = groups[service.id]["index"];
                    service.group = int.Parse(group);
                }

                service.isComment = items[id]["isComment"] == "True";
                service.isPriceManual = items[id]["isPriceManual"] == "True";

                servicesList.Add(service);
            }

            _services = servicesList;

            return true;
        }
    }
}
