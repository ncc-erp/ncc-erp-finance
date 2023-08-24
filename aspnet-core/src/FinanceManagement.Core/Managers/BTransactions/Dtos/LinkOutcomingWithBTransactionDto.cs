using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class LinkOutcomingWithBTransactionDto
    {
        public long BTransactionId { get; set; }
        public long ToBankAccountId { get; set; }
        public long OutcomingEntryId { get; set; }
        public double ExchangeRate { get; set; } = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE;
        public double? ToValue { get; set; }
        public bool BankTransactionNameEqualOutcomingName { get; set; } = false;
    }
    public class LinkMultipleOutcomingEntryWithBTransactionDto
    {
        public long BTransactionId { get; set; }
        public long ToBankAccountId { get; set; }
        public double ExchangeRate { get; set; } = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE;
        public List<long> OutcomingEntryIds { get; set; }
    }
    public class LinkOutcomingSalaryWithBTransactionsDto
    {
        public List<long> BTransactionIds { get; set; }
        public long ToBankAccountId { get; set; }
        public long OutcomingEntryId { get; set; }
        public double ExchangeRate { get; set; } = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE;
    }
}
