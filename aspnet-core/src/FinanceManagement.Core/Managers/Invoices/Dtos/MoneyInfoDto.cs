using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    public class MoneyInfoDto
    {
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string TotalMoney => CurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(TotalMoneyNumber) : Helpers.FormatMoney(TotalMoneyNumber);
        public double TotalMoneyNumber { get; set; }
    }
}
