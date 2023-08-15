using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntries.Dto
{
    public class GetTotalByCurrencyDto
    {
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public double TotalValue { get; set; }
        public double TotalValueToVND { get; set; }

    }
}
