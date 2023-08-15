using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BankAccounts.Dtos
{
    [AutoMapTo(typeof(BankAccount))]
    public class CreateBankAccountDto
    {
        public string HolderName { get; set; }
        public string BankNumber { get; set; }
        public long? BankId { get; set; }
        public long? CurrencyId { get; set; }
        public long AccountId { get; set; }
    }
}
