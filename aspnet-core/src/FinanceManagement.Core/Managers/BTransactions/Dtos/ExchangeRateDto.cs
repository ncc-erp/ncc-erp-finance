using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class CurrencyNeedConvertDto
    {
        public long FromCurrencyId { get; set; }
        public string FromCurrencyName { get; set; }
        public long ToCurrencyId { get; set; }
        public string ToCurrencyName { get; set; }
        public double ExchangeRate { get; set; } = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE;
        public bool IsReverseExchangeRate { get; set; }
    }
}
