using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Entities.NewEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class BankAccount : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string HolderName { get; set; }
        public string BankNumber { get; set; }
        public long? BankId { get; set; }
        [ForeignKey(nameof(BankId))]
        public virtual Bank Bank { get; set; }
        public long? CurrencyId { get; set; }
        [ForeignKey(nameof(CurrencyId))]
        public virtual Currency Currency { get; set; }
        public long AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; }
        public double Amount { get; set; }
        public double BaseBalance { get; set; }
        public bool LockedStatus { get; set; }
        public bool IsActive { get; set; } 

        public virtual ICollection<BTransaction> BTransactions { get; set; }
        public virtual ICollection<PeriodBankAccount> PeriodBankAccounts { get; set; }
    }
}

