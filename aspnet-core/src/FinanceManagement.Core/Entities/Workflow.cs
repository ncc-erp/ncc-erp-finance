using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Entities
{
    public class Workflow : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<WorkflowStatusTransition> WorkflowStatusTransitions { get; set; }
    }
   
}
