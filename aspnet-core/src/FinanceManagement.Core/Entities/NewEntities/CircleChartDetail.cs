using Abp.Domain.Entities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities.NewEntities
{
    public class CircleChartDetail : FKFullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long CircleChartId { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public string Color { get; set; }
        public long? BranchId { get; set; }
        public string ClientIds { get; set; }
        public string InOutcomeTypeIds { get; set; }

        #region Foreign Key
        [ForeignKey(nameof(CircleChartId))]
        public virtual CircleChart CircleChart { get; set; }
        [ForeignKey(nameof(BranchId))]
        public virtual Branch Branch { get; set; }
        #endregion
    }
}

