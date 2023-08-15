using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class LinkBankTransactionToBTransactionDto
    {
        public long BankTransactionId { get; set; }
        public long BTransactionId { get; set; }
        public double ExchangeRate { get; set; } = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE;
    }
}
