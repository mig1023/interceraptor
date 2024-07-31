using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrvFRLib;

namespace interceraptor.Cashbox
{
    class Setting
    {
        private DrvFR _driver { get; set; }

        private static int timeout = 159; // 1500ms

        private static Setting _singleton { get; set; }

        private Setting() { }

        public async static Task<Setting> Get()
        {
            if (_singleton == null)
            {
                _singleton = new Setting();
                var cashbox = await Cashbox.Connect.Get();
                _singleton._driver = cashbox.Driver;
            }

            return _singleton;
        }

        public bool Cashier(string cashier)
        {
            if (!SetField(2, 2, 1, cashier))
            {
                return false;
            }
            else if (!SetField(2, 2, 30, cashier))
            {
                return false;
            }
            else
            {
                return true;
            } 
        }

        private bool SetField(int table, int field, int row, string value)
        {
            _driver.TableNumber = table;
            _driver.FieldNumber = field;
            _driver.RowNumber = row;

            _driver.GetFieldStruct();
            _driver.ReadTable();
            _driver.Timeout = timeout;

            _driver.ValueOfFieldString = value;
            _driver.WriteTable();

            return _driver.ResultCode == 0;
        }
    }
}
