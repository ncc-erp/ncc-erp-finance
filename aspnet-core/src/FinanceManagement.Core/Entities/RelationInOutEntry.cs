using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class RelationInOutEntry : FullAuditedEntity<long>, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public int PeriodId { get; set; }
        public long OutcomingEntryId { get; set; }
        [ForeignKey(nameof(OutcomingEntryId))]
        public virtual OutcomingEntry OutcomingEntry { get; set; }
        public long IncomingEntryId { get; set; }
        [ForeignKey(nameof(IncomingEntryId))]
        public virtual IncomingEntry IncomingEntry { get; set; }
        /// <summary>
        /// Trong case bán đô -> add ghi nhận thu -> IsRefund = false
        /// Trong case chi 8M - Giao dịch ngân hàng 10M - ghi nhận thu 2M -> IsRefund = true
        /// </summary>
        public bool IsRefund { get; set; }
    }
}
