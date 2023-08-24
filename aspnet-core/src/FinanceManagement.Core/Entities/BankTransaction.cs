using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Entities
{
    public class BankTransaction : FullAuditedEntity<long>, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public long FromBankAccountId { get; set; }
        public long ToBankAccountId { get; set; }
        public double FromValue { get; set; }
        public double ToValue { get; set; }
        public double Fee { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; }
        public bool LockedStatus { get; set; }
        public long? BTransactionId { get; set; }
        [ForeignKey(nameof(BTransactionId))]
        public virtual BTransaction BTransaction { get; set; }

        public virtual ICollection<OutcomingEntryBankTransaction> OutcomingEntryBankTransactions { get; set; }
        public virtual ICollection<IncomingEntry> IncomingEntries { get; set; }
    }
}
