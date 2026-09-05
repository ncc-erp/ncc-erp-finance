using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntryReport.Dto
{
    public class IncomingTotalDto
    {
        public long EntryTypeId { get; set; }
        public long? CurrencyId { get; set; }
        public double TotalValue { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
