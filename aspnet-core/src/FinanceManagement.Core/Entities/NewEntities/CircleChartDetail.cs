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
        [ForeignKey(nameof(CircleChartId))]
        public long CircleChartId { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public string Color { get; set; }
        [ForeignKey(nameof(BranchId))]
        public long? BranchId { get; set; }
        public string ClientIds { get; set; }
        public string InOutcomingIds { get; set; }
        public bool isActive { get; set; }

        #region Foreign Key
        public virtual CircleChart CircleChart { get; set; }
        public virtual Branch Branch { get; set; }
        #endregion
    }
}

