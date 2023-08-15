using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class OutcomingEntrySupplier : FullAuditedEntity<long>, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public int PeriodId { get; set; }
        public long OutcomingEntryId { get; set; }
        [ForeignKey(nameof(OutcomingEntryId))]
        public virtual OutcomingEntry OutcomingEntry { get; set; }
        public long SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier Supplier { get; set; }
    }
}
