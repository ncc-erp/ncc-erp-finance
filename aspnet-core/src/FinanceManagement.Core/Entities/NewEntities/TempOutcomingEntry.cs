using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using FinanceManagement.Enums;
using Abp.AutoMapper;
using FinanceManagement.GeneralModels;

namespace FinanceManagement.Entities.NewEntities
{
    [AutoMapTo(typeof(OutcomingEntry))]
    public class TempOutcomingEntry : FullAuditedEntity<long>, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public long OutcomingEntryTypeId { get; set; }
        [ForeignKey(nameof(OutcomingEntryTypeId))]
        public virtual OutcomingEntryType OutcomingEntryType { get; set; }
        public double Value { get; set; }
        public long WorkflowStatusId { get; set; }
        [ForeignKey(nameof(WorkflowStatusId))]
        public virtual WorkflowStatus WorkflowStatus { get; set; }
        public long AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; }
        public long? BranchId { get; set; }
        [ForeignKey(nameof(BranchId))]
        public virtual Branch Branch { get; set; }
        public long? CurrencyId { get; set; }
        [ForeignKey(nameof(CurrencyId))]
        public virtual Currency Currency { get; set; }
        public DateTime? SentTime { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public DateTime? ExecutedTime { get; set; }
        [DefaultValue(0)]
        public string PaymentCode { get; set; }
        public Boolean Accreditation { get; set; }
        public OutcomingEntryFileStatus IsAcceptFile { get; set; }
        public bool IsOriginal { get; set; }

        public long RootOutcomingEntryId { get; set; }

        public string Reason { get; set; }
    }
}
