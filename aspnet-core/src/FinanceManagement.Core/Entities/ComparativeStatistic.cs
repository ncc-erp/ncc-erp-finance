using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Entities
{
    public class ComparativeStatistic : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DifferentExplanation { get; set; }
    }
}
