using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities.NewEntities
{
    public class PeriodBankAccount : FullAuditedEntity<long>, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public double BaseBalance { get; set; }
        public bool IsActive { get; set; } = true;
        #region relationship entity
        public int PeriodId { get; set; }
        [ForeignKey(nameof(PeriodId))]
        public virtual Period Period { get; set; }
        public long BankAccountId { get; set; }
        [ForeignKey(nameof(BankAccountId))]
        public virtual BankAccount BankAccount { get; set; }
        #endregion
    }
}
