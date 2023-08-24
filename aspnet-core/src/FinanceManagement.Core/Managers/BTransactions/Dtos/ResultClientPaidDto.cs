using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class ResultClientPaidDto
    {
        public string BankTransactionName { get; set; }
        public long AccountId { get; set; }
        public long BTransactionId { get; set; }
        public long BankAccountId { get; set; }
        public double Money { get; set; }
        public DateTime TimeAt { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
    }
}
