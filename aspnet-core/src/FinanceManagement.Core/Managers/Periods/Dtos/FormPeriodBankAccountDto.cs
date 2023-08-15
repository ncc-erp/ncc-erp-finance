using Abp.AutoMapper;
using Abp.Domain.Entities;
using FinanceManagement.Entities.NewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Periods.Dtos
{
    [AutoMapTo(typeof(PeriodBankAccount))]
    public class CreatePeriodBankAccountDto 
    {
        public long BankAccountId { get; set; }
        public double BaseBalance { get; set; }
    }
    public class CreatePeriodBankAccountTheFirstTime
    {
        public long BankAccountId { get; set; }
        public double CurrentBalance { get; set; }
    }

    [AutoMapTo(typeof (PeriodBankAccount))]
    public class EditPeriodBankAccountDto : Entity<long> 
    {
        public long BankAccountId { get; set; }
        public double BaseBalance { get; set; }
    }
}
