using System.Threading.Tasks;
using DrvFRLib;
using System;

namespace interceraptor.Cashbox
{
    class Connect
    {
        private DrvFR _driver { get; set; }

        public DrvFR Driver { get { return _driver; } }

        private static Connect _singleton { get; set; }

        public CashboxData Data { get; set; }
        
        public Connect() { } 

        public async static Task<Connect> Get()
        {
            if (_singleton == null)
            {
                _singleton = new Connect();

                string serial = await Task.Run(() => _singleton.GetCashboxDriver());
                _singleton.Data = new CashboxData { SerialNumber = serial };
            }

            return _singleton;
        }

        private string GetCashboxDriver()
        {
            _driver = new DrvFR();
    
            try
            {
                _driver.FindDevice();
                _driver.ReadSerialNumber();
            }
            catch
            {
                return null;
            }
            
            return _singleton._driver.SerialNumber;
        }

        public bool Check(out string error)
        {
            _driver.CheckConnection();

            if (_driver.ResultCode == 0)
            {
                error = String.Empty;
                return true;
            }
            else
            {
                error = _driver.ResultCodeDescription;
                return false;
            }
        }
    }
}
