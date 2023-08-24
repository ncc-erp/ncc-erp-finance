using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Commons.Dtos
{
    public class BTransDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Money{ get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyColor { get; set; }
        public DateTime TimeAt { get; set; }
        public string MoneyDisplay => (Money < 0 ? "" : "+") + (CurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(Money) : Helpers.FormatMoney(Money));
    }
}
