using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class Explanation : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string BankAccountExplanation { get; set; }
        public ExplanationType Type { get; set; }
        public long BankAccountId { get; set; }
        [ForeignKey(nameof(BankAccountId))]
        public virtual BankAccount BankAccount { get; set; }
        public long ComparativeStatisticId { get; set; }
        [ForeignKey(nameof(ComparativeStatisticId))]
        public virtual ComparativeStatistic ComparativeStatistic { get; set; }
    }
}
