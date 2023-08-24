using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities.NewEntities
{
    public class BTransaction : FKFullAuditedEntity, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public int PeriodId { get; set; }
        public long BankAccountId { get; set; }
        [ForeignKey(nameof(BankAccountId))]
        public virtual BankAccount BankAccount { get; set; }
        public double Money { get; set; }
        public DateTime TimeAt { get; set; }
        public long? FromAccountId { get; set; }
        [ForeignKey(nameof(FromAccountId))]
        public virtual Account FromAccount { get; set; }
        public string Note { get; set; }
        public BTransactionStatus Status { get; set; }
        public bool IsCrawl { get; set; }
        public virtual ICollection<IncomingEntry> IncomingEntries { get; set; }
    }
}
