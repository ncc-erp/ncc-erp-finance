using Abp.AutoMapper;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    [AutoMapTo(typeof(BTransaction))]
    public class CreateBTransactionDto
    {
        public long BankAccountId { get; set; }
        public double Money { get; set; }
        public DateTime TimeAt { get; set; }
        public string Note { get; set; }
        public BTransactionStatus Status { get; set; }
        public long BTransactionId { get; set; }
    }
}
