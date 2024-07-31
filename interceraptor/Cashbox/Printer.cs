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

        public Server.PayResponse Print(JObject doc)
        {

            //if (doc.Services.Count > 0 && doc.Services[0].ReturnShipping == 1)
            //    returnSale = true;

            PrepareDriver();

            //Driver.CheckType = (returnSale ? 2 : 0);
            _driver.CheckType = 0;

            _driver.OpenCheck();

            string customerCheck = String.Empty;

            if (!String.IsNullOrEmpty(doc["customer"]["email"].ToString()))
            {
                customerCheck = doc["customer"]["email"].ToString();
            }
            else if (!String.IsNullOrEmpty(doc["customer"]["tel"].ToString()))
            {
                customerCheck = doc["customer"]["tel"].ToString();
            }

            if (!String.IsNullOrEmpty(customerCheck))
            {
                _driver.CustomerEmail = customerCheck;
                _driver.FNSendCustomerEmail();
            }

            if (!String.IsNullOrEmpty(doc["agreement"]["barcode"].ToString()))
            {
                PrintLine("Договор: " + doc["agreement"]["barcode"].ToString());
            }

            foreach (var service in doc["agreement"]["services"])
            {
                _driver.Password = CashierPassword;
                _driver.Timeout = timeout;

                _driver.Quantity = int.Parse(service["qty"].ToString());
                _driver.Price = decimal.Parse(service["price"].ToString());

                string utf8 = Win1251toUTF8(service["name"].ToString());
                _driver.StringForPrinting = UTF8ToWin1251(utf8);

                _driver.Department = int.Parse(service["department"].ToString());

                _driver.Tax1 = int.Parse(service["taxGroup"].ToString());
                _driver.Tax2 = 0;
                _driver.Tax3 = 0;
                _driver.Tax4 = 0;

                _driver.PaymentItemSign = 4; // признак УСЛУГИ
                _driver.PaymentTypeSign = 4; // полный расчёт
                _driver.FNOperation();

                //AddAgentInfoByDepartment(service.Department);

                PrintLine(line: true);
            }

            PrepareDriver();

            _driver.StringForPrinting = String.Empty;

            var summ = decimal.Parse(doc["cashTotal"].ToString());

            if (doc["payMethod"].ToString() == "CREDIT_CARD")
            {
                //Log.Add("тип оплаты: безнал (реальный: " + doc.MoneyType.ToString() + ")");

                _driver.Summ2 = summ;
                _driver.Summ1 = 0;
            }
            else
            {
                //Log.Add("тип оплаты: наличными");

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

            /// tmp

            var response = new Server.PayResponse();

            //Log.AddWithCode("распечатка чека");

            if (checkClosingResult != 0)
            {
                PrepareDriver();

                _driver.CancelCheck();

                response.error = new Server.PayResponse.ErrorType
                {
                    type = "CASHDESK",
                    message = _driver.ResultCodeDescription,
                };

                //Log.AddWithCode("отмена чека");
            }
            else
            {
                //repeatPrintingTimer.Enabled = true;
                //repeatPrintingTimer.Start();

                response.checkNo = _driver.DocumentNumber.ToString();
                response.cashChange = _driver.Change.ToString();
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
