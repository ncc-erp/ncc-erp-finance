using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class RevenueManaged : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string NameInvoice { get; set; }
        public long? AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; }
        public short Month { get; set; }
        public double CollectionDebt { get; set; }
        public double? DebtReceived { get; set; }
        public long UnitId { get; set; }
        [ForeignKey(nameof(UnitId))]
        public virtual Currency Currency { get; set; }
        public RevenueManagedStatus Status { get; set; }
        public DateTime SendInvoiceDate { get; set; }
        public DateTime Deadline { get; set; }
        public RemindStatus? RemindStatus { get; set; }
        public string Note { get; set; }
    }
}
