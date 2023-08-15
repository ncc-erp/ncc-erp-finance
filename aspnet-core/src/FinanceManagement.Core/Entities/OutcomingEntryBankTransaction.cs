using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class OutcomingEntryBankTransaction : FullAuditedEntity<long>, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public int PeriodId { get; set; }
        public long BankTransactionId { get; set; }
        [ForeignKey(nameof(BankTransactionId))]
        public virtual BankTransaction BankTransaction { get; set; }
        public long OutcomingEntryId { get; set; }
        [ForeignKey(nameof(OutcomingEntryId))]
        public virtual OutcomingEntry OutcomingEntry { get; set; }

    }
}
