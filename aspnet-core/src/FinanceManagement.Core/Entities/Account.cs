using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class Account : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long AccountTypeId { get; set; }
        [ForeignKey(nameof(AccountTypeId))]
        public virtual AccountType AccountType { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Default { get; set; }
        public bool IsActive { get; set; }
        public AccountTypeEnum Type { get; set; }

        public virtual ICollection<RevenueManaged> RevenueManageds { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
    }
}
