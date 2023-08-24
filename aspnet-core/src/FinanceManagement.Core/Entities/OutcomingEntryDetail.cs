using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    [AutoMapTo(typeof(TempOutcomingEntryDetail))]
    public class OutcomingEntryDetail : FullAuditedEntity<long>, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public int PeriodId { get; set; }
        public long? AccountId { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
        public long OutcomingEntryId { get; set; }
        [ForeignKey(nameof(OutcomingEntryId))]
        public virtual OutcomingEntry OutcomingEntry { get; set; }
        public long BranchId { get; set; }
        [ForeignKey(nameof(BranchId))]
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// false - đã hoàn thành, đã chuyển lương cho nhanh viên trong Request chi Bảng lương
        /// true - chưa chuyển lương cho nhân viên
        /// </summary>
        public bool IsNotDone { get; set; } = false;
    }
}
