using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.IncomingEntries.Dtos
{
    public class IncomingEntryGridParam: GridParam
    {
        public long? Id { get; set; }
        public List<long> ClientAccountIds { get; set; }
        public double? Money { get; set; }
        public long? IncomingEntryTypeId { get; set; }
        public long? CurrencyId { get; set; }
        public bool? RevenueCounted { get; set; }
        public IncomingEntryFilterDateTime FilterDateTimeParam { get; set; }
    }
    public class IncomingEntryFilterDateTime : FilterDateTimeParam 
    {
        public IncomingEntryFilterDateTimeType DateTimeType { get; set; }
    }
    public enum IncomingEntryFilterDateTimeType
    {
        NO_FILTER = 0,
        CREATION_TIME = 1,
        TRANSACTION_TIME = 2,
        UPDATED_TIME = 3,
    }

}
