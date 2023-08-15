using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class RevenueManagedFile : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long RevenueManagedId { get; set; }
        [ForeignKey(nameof(RevenueManagedId))]
        public virtual RevenueManaged RevenueManaged { get; set; }
        public string FilePath { get; set; }

    }
}
