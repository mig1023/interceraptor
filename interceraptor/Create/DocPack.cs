using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public decimal ParseDecimal(string line)
        {
            double decimalTemporary = 0;

            line = line.Replace(",", ".");

            if (line == String.Empty)
                decimalTemporary = 0;
            else
                Double.TryParse(line, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalTemporary);

            return (decimal)decimalTemporary;
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

        public async Task<Server.PayResponse> Print(string moneyLine, string email, string payMethod)
        {
            decimal total = 0;
            
            foreach (var service in _servicesPriced)
            {
                total += service.price * service.qty;
            }

            decimal money;

            if (payMethod == "CREDIT_CARD")
            {
                money = total;
            }
            else
            {
                if (!decimal.TryParse(moneyLine, out money))
                {
                    return new Server.PayResponse
                    {
                        error = new Server.PayResponse.ErrorType
                        {
                            message = "Не указана или не верно указана сумма наличных"
                        }
                    };
                }
            }

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
                cashTotal = money,
                payMethod = payMethod,
            });

            Cashbox.Printer cashbox = Cashbox.Printer.Get();
            return cashbox.Print(docPack, noCorr: true);
        }
    }
}
