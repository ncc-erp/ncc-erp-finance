using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Helper;


namespace FinanceManagement.APIs.IncomingEntryReport.Dto
{
    public class GetTotalIncomingCurrencyDto
    {
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public double TotalValue { get; set; }
        public string ValueFormat => Helpers.FormatMoney(TotalValue);
    }
}
