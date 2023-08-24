using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BankTransactions.Dtos
{
    [AutoMapTo(typeof(BankTransaction))]
    public class CreateBankTransactionDto
    {
        public string Name { get; set; }
        public long FromBankAccountId { get; set; }
        public long ToBankAccountId { get; set; }
        public double FromValue { get; set; }
        public double ToValue { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; }
        public long? BTransactionId { get; set; }
    }
}
