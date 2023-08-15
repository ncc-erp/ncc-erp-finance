using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class LinkBTransactionInfomationDto
    {
        public long BankAccountId { get; set; }
        public double Money { get; set; }
        public DateTime TimeAt { get; set; }
        public BTransactionStatus Status { get; set; }
        public string BankNumber { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public long? FromAccountId { get; set; }
    }
}
