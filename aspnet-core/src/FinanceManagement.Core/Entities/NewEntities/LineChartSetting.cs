using Abp.Domain.Entities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities.NewEntities
{
    public class LineChartSetting : FKFullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(LinechartId))]
        public long LinechartId { get; set; }
        public virtual LineChart LineChart { get; set; }
        public long ReferenceId { get; set; }
    }
}
