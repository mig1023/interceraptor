using System.Threading.Tasks;
using DrvFRLib;

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
            _driver.FindDevice();
    
            _driver.ReadSerialNumber();
            return _singleton._driver.SerialNumber;
        }

        public bool Check()
        {
            _driver.CheckConnection();
            return _driver.ResultCode == 0;
        }
    }
}
