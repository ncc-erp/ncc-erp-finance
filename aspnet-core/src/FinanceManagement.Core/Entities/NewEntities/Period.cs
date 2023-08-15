using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Entities.NewEntities
{
    public class Period : FullAuditedEntity<int>, IMayHaveTenant
    {
        [MaxLength(200)]
        public string Name { get; set; }
        public int? TenantId { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        #region Relationship Entites
        public virtual ICollection<PeriodBankAccount> PeriodBankAccounts { get; private set; }
        #endregion
    }
}
