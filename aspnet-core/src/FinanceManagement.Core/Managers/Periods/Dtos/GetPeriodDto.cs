using Abp.Domain.Entities;
using FinanceManagement.Anotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Periods.Dtos
{
    public class GetPeriodDto : Entity<int>
    {
        [ApplySearchAttribute]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreateByUserName { get; set; }
        public string StatusName => IsActive ? "Active" : "InActive";
    }
    public class GetPeriodHaveDetail : GetPeriodDto
    {
        public IEnumerable<GetPeriodBankAccount> PeriodBankAccounts { get; set; }
    }
    public class GetPeriodBankAccount : Entity<long>
    {
        public long BankAccountId { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public double BaseBalance { get; set; }
    }
}
