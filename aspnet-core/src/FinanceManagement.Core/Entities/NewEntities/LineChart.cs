using Abp.Domain.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Entities.NewEntities
{
    public class LineChart : FKFullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public LineChartSettingType Type { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(50)]
        public string Color { get; set; }

        public virtual ICollection<LineChartSetting> LineChartSettings { get; set; }
    }
}
