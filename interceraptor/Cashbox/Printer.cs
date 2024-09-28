using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrvFRLib;
using Newtonsoft.Json.Linq;

namespace interceraptor.Cashbox
{
    class Printer
    {
        private DrvFR _driver { get; set; }

        static int CashierPassword = 1;
        static int AdminPassword = 30;
        public static int timeout = 159; // 1500ms

        private static Printer _singleton { get; set; }

        private Printer() { }

        public static Printer Get()
        {
            if (_singleton == null)
            {
                _singleton = new Printer();
                var cashbox = Cashbox.Connect.Get().GetAwaiter().GetResult();
                _singleton._driver = cashbox.Driver;
            }
                
            return _singleton;
        }

        public void PrepareDriver(bool admin = false)
        {
            _driver.Password = (admin ? AdminPassword : CashierPassword);
            _driver.Timeout = timeout;
        }

        public void PrintLine(string text = "", bool line = false, int password = 0)
        {
            PrepareDriver();

            if (!String.IsNullOrEmpty(text))
            {
                _driver.StringForPrinting = text;
                _driver.PrintString();
            }

            if (line)
            {
                _driver.StringForPrinting = new String('-', 36);
                _driver.PrintString();
            }
        }

        private void AddAgentInfo(string name, string phone, string inn)
        {
            _driver.TagNumber = 1222;
            _driver.TagType = 0;
            _driver.TagValueInt = 16;   // 16  --> 00000100 - 5 бит c нулевого (поверенный)
            _driver.FNSendTagOperation();

            int my_TagID = _driver.TagID;

            _driver.TagNumber = 1224;
            _driver.FNBeginSTLVTag();

            _driver.TagID = my_TagID;
            _driver.TagNumber = 1225;
            _driver.TagType = 7;
            _driver.TagValueStr = name;
            _driver.FNAddTag();

            _driver.TagID = my_TagID;
            _driver.TagNumber = 1171;
            _driver.TagType = 7;
            _driver.TagValueStr = phone;
            _driver.FNAddTag();

            _driver.FNSendSTLVTagOperation();

            _driver.TagNumber = 1226;
            _driver.TagType = 7;
            _driver.TagValueStr = inn;
            _driver.FNSendTagOperation();
        }

        private void AddAgentInfoByDepartment(int department)
        {
            if (department == 2)
                AddAgentInfo("КОНСУЛЬСТВО", "+74951234567", "74951234567");
            else if (department == 4)
                AddAgentInfo("СТРАХОВКА 1", "+74951234567", "74951234567");
            else if (department == 5)
                AddAgentInfo("СТРАХОВКА 1", "+74951234567", "74951234567");
        }

        public Server.PayResponse Print(JObject doc, bool noCorr = false)
        {

            //if (doc.Services.Count > 0 && doc.Services[0].ReturnShipping == 1)
            //    returnSale = true;

            PrepareDriver();

            //Driver.CheckType = (returnSale ? 2 : 0);
            _driver.CheckType = 0;

            _driver.OpenCheck();

            string customerCheck = String.Empty;
            string email = doc["customer"]?["email"]?.ToString() ?? String.Empty;
            string tel = doc["customer"]?["tel"]?.ToString() ?? String.Empty;

            if (!String.IsNullOrEmpty(email))
            {
                customerCheck = email;
            }
            else if (!String.IsNullOrEmpty(tel))
            {
                customerCheck = tel;
            }

            if (!String.IsNullOrEmpty(customerCheck))
            {
                _driver.CustomerEmail = customerCheck;
                _driver.FNSendCustomerEmail();
            }

            string barcode = doc["agreement"]["barcode"]?.ToString() ?? String.Empty;

            if (!String.IsNullOrEmpty(barcode))
            {
                PrintLine($"Договор: {barcode}");
            }

            foreach (var service in doc["agreement"]["services"])
            {
                _driver.Password = CashierPassword;
                _driver.Timeout = timeout;

                _driver.Quantity = int.Parse(service["qty"].ToString());
                _driver.Price = decimal.Parse(service["price"].ToString());

                if (noCorr)
                {
                    _driver.StringForPrinting = service["name"].ToString();
                }
                else
                {
                    string utf8 = Win1251toUTF8(service["name"].ToString());
                    _driver.StringForPrinting = UTF8ToWin1251(utf8);
                }
                

                var department = int.Parse(service["department"].ToString());
                _driver.Department = department;

                _driver.Tax1 = int.Parse(service["taxGroup"].ToString());
                _driver.Tax2 = 0;
                _driver.Tax3 = 0;
                _driver.Tax4 = 0;

                _driver.PaymentItemSign = 4;
                _driver.PaymentTypeSign = 4;
                _driver.FNOperation();

                AddAgentInfoByDepartment(department);

                PrintLine(line: true);
            }

            PrepareDriver();

            _driver.StringForPrinting = String.Empty;

            var summ = decimal.Parse(doc["cashTotal"].ToString());

            if (doc["payMethod"].ToString() == "CREDIT_CARD")
            {
                _driver.Summ2 = summ;
                _driver.Summ1 = 0;
            }
            else
            {
                _driver.Summ1 = summ;
                _driver.Summ2 = 0;
            }

            // данные о чеке, который корректируется
            //if (!String.IsNullOrEmpty(fdCorrection))
            //{
            //    AddCorrectionFDNum(fdCorrection);
            //}

            _driver.FNCloseCheckEx();

            int checkClosingResult = _driver.ResultCode;
            string checkClosingErrorText = _driver.ResultCodeDescription;

            /// printing

            var response = new Server.PayResponse();

            if (checkClosingResult != 0)
            {
                PrepareDriver();

                _driver.CancelCheck();

                response.error = new Server.PayResponse.ErrorType
                {
                    type = "CASHDESK",
                    message = checkClosingErrorText,
                };
            }
            else
            {
                //repeatPrintingTimer.Enabled = true;
                //repeatPrintingTimer.Start();

                response.checkNo = _driver.DocumentNumber.ToString();
                response.cashChange = _driver.Change.ToString().Replace(',', '.');
            }

            return response;
        }

        private string Win1251toUTF8(string sourceStr)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] utf8Bytes = win1251.GetBytes(sourceStr);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
            return win1251.GetString(win1251Bytes);
        }

        private string UTF8ToWin1251(string sourceStr)
        {
            Encoding utf8 = Encoding.UTF8;
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] utf8Bytes = utf8.GetBytes(sourceStr);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
            return win1251.GetString(win1251Bytes);
        }
    }
}
