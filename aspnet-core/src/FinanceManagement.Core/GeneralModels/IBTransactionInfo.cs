using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public interface IBTransactionInfo
    {
        public long? BTransactionId { get; set; }
        public DateTime? BTransactionTimeAt { get; set; }
        public string BTransactionMoney { get; }
        public double? BTransactionMoneyNumber { get; set; }
        public string BTransactionBankNumber { get; set; }
        public long? BTransactionCurrencyId{ get; set; }
        public string BTransactionCurrencyName { get; set; }
    }
}
